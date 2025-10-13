//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Esper.FeelSpeak.Database
{
    /// <summary>
    /// Feel Speak's database handler. Executes all database queries.
    /// </summary>
    public static class FeelSpeakDatabase
    {
        /// <summary>
        /// The database directory path in the editor.
        /// </summary>
        public static string DatabaseEditorDirectoryPath { get; private set; } = Application.streamingAssetsPath;

        /// <summary>
        /// The database directory path at runtime.
        /// </summary>
        public static string DatabaseRuntimeDirectoryPath { get; private set; } = Application.persistentDataPath;

        public static string databaseName;
        private readonly static string dialogueTableName = "dialogue_graphs";
        private readonly static string characterTableName = "characters";
        private readonly static string emotionTableName = "emotions";

        private static SQLiteConnection connection;

        /// <summary>
        /// If there is an active connection to the database.
        /// </summary>
        public static bool IsConnected { get => connection != null; }

        /// <summary>
        /// Initializes the database connection.
        /// </summary>
        public static void Initialize()
        {
            if (IsConnected)
            {
                return;
            }

            string databaseFullPath = string.Empty;

            bool updateDatabase = false;
            databaseName = $"{FeelSpeak.Settings.databaseName}.db";

#if UNITY_EDITOR
            // Always connect to editor database if in the Unity editor
            databaseFullPath = Path.Combine(DatabaseEditorDirectoryPath, databaseName);

            // Create database directory if it does not exists
            if (!Directory.Exists(DatabaseEditorDirectoryPath))
            {
                Directory.CreateDirectory(DatabaseEditorDirectoryPath);
            }

            // Create database file if it does not exist
            if (!File.Exists(databaseFullPath))
            {
                File.WriteAllBytes(databaseFullPath, new byte[] { });
                updateDatabase = true;
            }
#else
            // Connect to runtime database if not in the Unity editor
            databaseFullPath = Path.Combine(DatabaseRuntimeDirectoryPath, databaseName);

            // Create database directory if it does not exist
            if (!Directory.Exists(DatabaseRuntimeDirectoryPath))
            {
                Directory.CreateDirectory(DatabaseRuntimeDirectoryPath);
            }

            if (!File.Exists(databaseFullPath))
            {
                CreateRuntimeDatabase();
            }

            // Always update database in case of changes made
            updateDatabase = true;
#endif

            connection = new SQLiteConnection(databaseFullPath);

            // Create tables if they don't exist
            CreateDialogueTable();
            CreateCharacterTable();
            CreateEmotionTable();

            if (updateDatabase)
            {
                // Clear tables to ensure deleted objects are removed
                ClearTable(dialogueTableName);
                ClearTable(characterTableName);
                ClearTable(emotionTableName);

                var graphs = FeelSpeak.GetAllDialogueGraphs();

                foreach (var item in graphs)
                {
                    item.UpdateDatabaseRecord();
                }

                var characters = FeelSpeak.GetAllCharacters();

                foreach (var item in characters)
                {
                    item.UpdateDatabaseRecord();
                }

                var emotions = FeelSpeak.GetAllEmotions();

                foreach (var item in emotions)
                {
                    item.UpdateDatabaseRecord();
                }

                graphs = null;
                characters = null;
                emotions = null;

                Resources.UnloadUnusedAssets();
            }

            if (Application.isPlaying)
            {
                FeelSpeakLogger.Log("Feel Speak: Initialized database connection.");
            }
        }

        /// <summary>
        /// Disconnects from the database.
        /// </summary>
        public static void Disconnect()
        {
            if (!IsConnected)
            {
                return;
            }

            connection.Close();
            connection.Dispose();
            connection = null;

            if (Application.isPlaying)
            {
                DeleteRuntimeDatabase();
                FeelSpeakLogger.Log("Feel Speak: Disconnected from database.");
            }
        }

        /// <summary>
        /// Copies the database from StreamingAssets and stores it in the user's system. This is
        /// necessary to have a connection with the database at runtime.
        /// </summary>
        public static void CreateRuntimeDatabase()
        {
            string databaseEditorFullPath = Path.Combine(DatabaseEditorDirectoryPath, databaseName);
            string databaseRuntimeFullPath = Path.Combine(DatabaseRuntimeDirectoryPath, databaseName);

            string uri = string.Empty;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
            uri = "file://" + databaseEditorFullPath;
#else
            uri = databaseEditorFullPath;
#endif

            // Copy file from default database (from StreamingAssets)
            var loadingRequest = UnityWebRequest.Get(uri);
            loadingRequest.SendWebRequest();

            while (!loadingRequest.isDone)
            {
                switch (loadingRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        FeelSpeakLogger.LogError("Feel Speak: Failed to create runtime database (connection error).");
                        return;

                    case UnityWebRequest.Result.ProtocolError:
                        FeelSpeakLogger.LogError("Feel Speak: Failed to create runtime database (protocol error).");
                        return;

                    case UnityWebRequest.Result.DataProcessingError:
                        FeelSpeakLogger.LogError("Feel Speak: Failed to create runtime database (data processing error).");
                        return;
                }
            }

            if (loadingRequest.downloadHandler.data == null || loadingRequest.downloadHandler.data.Length == 0)
            {
                FeelSpeakLogger.LogError("Feel Speak: Failed to create runtime database. This usually means that the editor " +
                    "database has not been created or found. Try navigating to Window > Feel Speak > Settings, click " +
                    "'Validate', and then try again. If this persists, consider contacting the developer.");
                return;
            }

            File.WriteAllBytes(databaseRuntimeFullPath, loadingRequest.downloadHandler.data);
            loadingRequest.Dispose();
        }


        /// <summary>
        /// Deletes the runtime database. This requires a disconnection first.
        /// </summary>
        public static void DeleteRuntimeDatabase()
        {
            var dbName = string.IsNullOrEmpty(databaseName) ? FeelSpeak.Settings.databaseName : databaseName;
            string databaseFullPath = Path.Combine(DatabaseRuntimeDirectoryPath, dbName);
            File.Delete(databaseFullPath);
        }

        /// <summary>
        /// Deletes the editor database. This requires a disconnection first.
        /// </summary>
        public static void DeleteEditorDatabase()
        {
            var dbName = string.IsNullOrEmpty(databaseName) ? FeelSpeak.Settings.databaseName : databaseName;
            string databaseFullPath = Path.Combine(DatabaseEditorDirectoryPath, dbName);
            File.Delete(databaseFullPath);
            File.Delete($"{databaseFullPath}.meta");
        }

        /// <summary>
        /// Deletes all records from a table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        public static void ClearTable(string tableName)
        {
            connection.Execute($"DELETE FROM {tableName}");
        }

        /// <summary>
        /// Gets the total number of records in a table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The total number of records.</returns>
        public static int GetRecordCount(string tableName)
        {
            int count = connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM {tableName}");
            return count;
        }

        /// <summary>
        /// Creates the dialogue table if it doesn't exist.
        /// </summary>
        public static void CreateDialogueTable()
        {
            connection.Execute($"CREATE TABLE IF NOT EXISTS {dialogueTableName} (id INTEGER NOT NULL PRIMARY KEY, object_name VARCHAR(255), graph_name VARCHAR(255))");
        }

        /// <summary>
        /// Inserts a dialogue record.
        /// </summary>
        /// <param name="dialogueRecord">The dialogue record.</param>
        public static void InsertDialogueRecord(DialogueRecord dialogueRecord)
        {
            var cmd = connection.CreateCommand($"INSERT INTO {dialogueTableName} VALUES (@id, @object_name, @graph_name)");
            cmd.Bind("@id", dialogueRecord.id);
            cmd.Bind("@object_name", dialogueRecord.objectName);
            cmd.Bind("@graph_name", dialogueRecord.graphName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates an existing dialogue record.
        /// </summary>
        /// <param name="dialogueRecord">The dialogue record.</param>
        public static void UpdateDialogueRecord(DialogueRecord dialogueRecord)
        {
            var cmd = connection.CreateCommand($"UPDATE {dialogueTableName} SET object_name = @object_name, graph_name = @graph_name WHERE id = @id");
            cmd.Bind("@id", dialogueRecord.id);
            cmd.Bind("@object_name", dialogueRecord.objectName);
            cmd.Bind("@graph_name", dialogueRecord.graphName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes an existing dialogue record.
        /// </summary>
        /// <param name="dialogueRecord">The dialogue record.</param>
        public static void DeleteDialogueRecord(DialogueRecord dialogueRecord)
        {
            var cmd = connection.CreateCommand($"DELETE FROM {dialogueTableName} WHERE id = @id");
            cmd.Bind("@id", dialogueRecord.id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes an existing dialogue record.
        /// </summary>
        /// <param name="id">The dialogue record ID.</param>
        public static void DeleteDialogueRecord(int id)
        {
            var cmd = connection.CreateCommand($"DELETE FROM {dialogueTableName} WHERE id = @id");
            cmd.Bind("@id", id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Checks if a dialogue record exists.
        /// </summary>
        /// <param name="id">The record ID.</param>
        /// <returns>True if the record exists. Otherwise, false.</returns>
        public static bool HasDialogueRecord(int id)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {dialogueTableName} WHERE id = @id");
            cmd.Bind("@id", id);

            var records = cmd.ExecuteQuery<DialogueRecord>();

            return records.Count > 0;
        }

        /// <summary>
        /// Gets a dialogue record.
        /// </summary>
        /// <param name="id">The dialogue ID.</param>
        /// <returns>A dialogue record with the ID or null if it doesn't exist.</returns>
        public static DialogueRecord GetDialogueRecord(int id)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {dialogueTableName} WHERE id = @id");
            cmd.Bind("@id", id);

            var records = cmd.ExecuteQuery<DialogueRecord>();

            if (records.Count > 0)
            {
                return records.First();
            }

            return null;
        }

        /// <summary>
        /// Gets a dialogue record.
        /// </summary>
        /// <param name="dialogueName">The dialogue dialogue name.</param>
        /// <returns>A dialogue record with the ID or null if it doesn't exist.</returns>
        public static DialogueRecord GetDialogueRecord(string dialogueName)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {dialogueTableName} WHERE LOWER(graph_name) = LOWER(@graph_name)");
            cmd.Bind("@graph_name", dialogueName);

            var records = cmd.ExecuteQuery<DialogueRecord>();

            if (records.Count > 0)
            {
                return records.First();
            }

            return null;
        }

        /// <summary>
        /// Gets all dialogue records that are a part of a specific dialogue group.
        /// </summary>
        /// <param name="groupId">The dialogue group ID.</param>
        /// <returns>All dialogue records with the group ID.</returns>
        public static List<DialogueRecord> GetDialogueRecords(int groupId)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {dialogueTableName} WHERE group_id = @group_id");
            cmd.Bind("@group_id", groupId);

            var records = cmd.ExecuteQuery<DialogueRecord>();
            return records.ToList();
        }

        /// <summary>
        /// Gets all dialogue records within a range and only includes those that contain a given string.
        /// </summary>
        /// <param name="min">The min range.</param>
        /// <param name="max">The max range.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="desc">If descending order by mode should be used.</param>
        /// <returns>All dialogue records within range filtered by the pattern.</returns>
        public static List<DialogueRecord> GetDialogueRecords(int min, int max, string pattern, bool desc)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {dialogueTableName} WHERE LOWER(graph_name) LIKE LOWER(@pattern) ORDER BY LOWER(graph_name) {(desc ? "DESC" : "ASC")} LIMIT @max OFFSET @min");
            cmd.Bind("@min", min);
            cmd.Bind("@max", max);
            cmd.Bind("@pattern", $"%{pattern}%");

            var records = cmd.ExecuteQuery<DialogueRecord>();
            return records.ToList();
        }

        /// <summary>
        /// Gets all dialogue records that contain a given string.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="desc">If descending order by mode should be used.</param>
        /// <returns>All dialogue records within range filtered by the pattern.</returns>
        public static List<DialogueRecord> GetDialogueRecords(string pattern, bool desc)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {dialogueTableName} WHERE LOWER(graph_name) LIKE LOWER(@pattern) ORDER BY LOWER(graph_name) {(desc ? "DESC" : "ASC")}");
            cmd.Bind("@pattern", $"%{pattern}%");

            var records = cmd.ExecuteQuery<DialogueRecord>();
            return records.ToList();
        }

        /// <summary>
        /// Gets the total number of records in the dialogue table.
        /// </summary>
        /// <returns>The total number of records.</returns>
        public static int GetDialogueCount()
        {
            return GetRecordCount(dialogueTableName);
        }

        /// <summary>
        /// Gets the total number of records in the dialogue table filtered by the pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>The total number of filtered records.</returns>
        public static int GetFilteredDialogueCount(string pattern)
        {
            int count = connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM {dialogueTableName} WHERE LOWER(graph_name) LIKE LOWER(?)", $"%{pattern}%");
            return count;
        }

        /// <summary>
        /// Creates the character table if it doesn't exist.
        /// </summary>
        public static void CreateCharacterTable()
        {
            connection.Execute($"CREATE TABLE IF NOT EXISTS {characterTableName} (id INTEGER NOT NULL PRIMARY KEY, object_name VARCHAR(255), character_name TEXT)");
        }

        /// <summary>
        /// Inserts a character record.
        /// </summary>
        /// <param name="characterRecord">The character record.</param>
        public static void InsertCharacterRecord(CharacterRecord characterRecord)
        {
            var cmd = connection.CreateCommand($"INSERT INTO {characterTableName} VALUES (@id, @object_name, @character_name)");
            cmd.Bind("@id", characterRecord.id);
            cmd.Bind("@object_name", characterRecord.objectName);
            cmd.Bind("@character_name", characterRecord.characterName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates an existing character record.
        /// </summary>
        /// <param name="characterRecord">The character record.</param>
        public static void UpdateCharacterRecord(CharacterRecord characterRecord)
        {
            var cmd = connection.CreateCommand($"UPDATE {characterTableName} SET object_name = @object_name, character_name = @character_name WHERE id = @id");
            cmd.Bind("@id", characterRecord.id);
            cmd.Bind("@object_name", characterRecord.objectName);
            cmd.Bind("@character_name", characterRecord.characterName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes an existing character record.
        /// </summary>
        /// <param name="characterRecord">The character record.</param>
        public static void DeleteCharacterRecord(CharacterRecord characterRecord)
        {
            var cmd = connection.CreateCommand($"DELETE FROM {characterTableName} WHERE id = @id");
            cmd.Bind("@id", characterRecord.id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes an existing character record.
        /// </summary>
        /// <param name="id">The character record ID.</param>
        public static void DeleteCharacterRecord(int id)
        {
            var cmd = connection.CreateCommand($"DELETE FROM {characterTableName} WHERE id = @id");
            cmd.Bind("@id", id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Checks if a character record exists.
        /// </summary>
        /// <param name="id">The record ID.</param>
        /// <returns>True if the record exists. Otherwise, false.</returns>
        public static bool HasCharacterRecord(int id)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {characterTableName} WHERE id = @id");
            cmd.Bind("@id", id);

            var records = cmd.ExecuteQuery<CharacterRecord>();

            return records.Count > 0;
        }

        /// <summary>
        /// Gets a character record.
        /// </summary>
        /// <param name="id">The character ID.</param>
        /// <returns>A character record with the ID or null if it doesn't exist.</returns>
        public static CharacterRecord GetCharacterRecord(int id)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {characterTableName} WHERE id = @id");
            cmd.Bind("@id", id);

            var records = cmd.ExecuteQuery<CharacterRecord>();

            if (records.Count > 0)
            {
                return records.First();
            }

            return null;
        }

        /// <summary>
        /// Gets a character record.
        /// </summary>
        /// <param name="characterName">The character name.</param>
        /// <returns>A character record with the ID or null if it doesn't exist.</returns>
        public static CharacterRecord GetCharacterRecord(string characterName)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {characterTableName} WHERE LOWER(character_name) = LOWER(@character_name)");
            cmd.Bind("@character_name", characterName);

            var records = cmd.ExecuteQuery<CharacterRecord>();

            if (records.Count > 0)
            {
                return records.First();
            }

            return null;
        }

        /// <summary>
        /// Gets all character records within a range and only includes those that contain a given string.
        /// </summary>
        /// <param name="min">The min range.</param>
        /// <param name="max">The max range.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="desc">If descending order by mode should be used.</param>
        /// <returns>All character records within range filtered by the pattern.</returns>
        public static List<CharacterRecord> GetCharacterRecords(int min, int max, string pattern, bool desc)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {characterTableName} WHERE LOWER(character_name) LIKE LOWER(@pattern) ORDER BY LOWER(character_name) {(desc ? "DESC" : "ASC")} LIMIT @max OFFSET @min");
            cmd.Bind("@min", min);
            cmd.Bind("@max", max);
            cmd.Bind("@pattern", $"%{pattern}%");

            var records = cmd.ExecuteQuery<CharacterRecord>();
            return records.ToList();
        }

        /// <summary>
        /// Gets all character records that contain a given string.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="desc">If descending order by mode should be used.</param>
        /// <returns>All character records within range filtered by the pattern.</returns>
        public static List<CharacterRecord> GetCharacterRecords(string pattern, bool desc)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {characterTableName} WHERE LOWER(character_name) LIKE LOWER(@pattern) ORDER BY LOWER(character_name) {(desc ? "DESC" : "ASC")}");
            cmd.Bind("@pattern", $"%{pattern}%");

            var records = cmd.ExecuteQuery<CharacterRecord>();
            return records.ToList();
        }

        /// <summary>
        /// Gets the total number of records in the character table.
        /// </summary>
        /// <returns>The total number of records.</returns>
        public static int GetCharacterCount()
        {
            return GetRecordCount(characterTableName);
        }

        /// <summary>
        /// Gets the total number of records in the character table filtered by the pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>The total number of filtered records.</returns>
        public static int GetFilteredCharacterCount(string pattern)
        {
            int count = connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM {characterTableName} WHERE LOWER(character_name) LIKE LOWER(?)", $"%{pattern}%");
            return count;
        }

        /// <summary>
        /// Creates the emotion table if it doesn't exist.
        /// </summary>
        public static void CreateEmotionTable()
        {
            connection.Execute($"CREATE TABLE IF NOT EXISTS {emotionTableName} (id INTEGER NOT NULL PRIMARY KEY, object_name VARCHAR(255), emotion_name TEXT)");
        }

        /// <summary>
        /// Inserts an emotion record.
        /// </summary>
        /// <param name="emotionRecord">The emotion record.</param>
        public static void InsertEmotionRecord(EmotionRecord emotionRecord)
        {
            var cmd = connection.CreateCommand($"INSERT INTO {emotionTableName} VALUES (@id, @object_name, @emotion_name)");
            cmd.Bind("@id", emotionRecord.id);
            cmd.Bind("@object_name", emotionRecord.objectName);
            cmd.Bind("@emotion_name", emotionRecord.emotionName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates an existing emotion record.
        /// </summary>
        /// <param name="emotionRecord">The emotion record.</param>
        public static void UpdateEmotionRecord(EmotionRecord emotionRecord)
        {
            var cmd = connection.CreateCommand($"UPDATE {emotionTableName} SET object_name = @object_name, emotion_name = @emotion_name WHERE id = @id");
            cmd.Bind("@id", emotionRecord.id);
            cmd.Bind("@object_name", emotionRecord.objectName);
            cmd.Bind("@emotion_name", emotionRecord.emotionName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes an existing emotion record.
        /// </summary>
        /// <param name="emotionRecord">The emotion record.</param>
        public static void DeleteEmotionRecord(EmotionRecord emotionRecord)
        {
            var cmd = connection.CreateCommand($"DELETE FROM {emotionTableName} WHERE id = @id");
            cmd.Bind("@id", emotionRecord.id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes an existing emotion record.
        /// </summary>
        /// <param name="id">The emotion record ID.</param>
        public static void DeleteEmotionRecord(int id)
        {
            var cmd = connection.CreateCommand($"DELETE FROM {emotionTableName} WHERE id = @id");
            cmd.Bind("@id", id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Checks if an emotion record exists.
        /// </summary>
        /// <param name="id">The record ID.</param>
        /// <returns>True if the record exists. Otherwise, false.</returns>
        public static bool HasEmotionRecord(int id)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {emotionTableName} WHERE id = @id");
            cmd.Bind("@id", id);

            var records = cmd.ExecuteQuery<EmotionRecord>();

            return records.Count > 0;
        }

        /// <summary>
        /// Gets an emotion record.
        /// </summary>
        /// <param name="id">The emotion ID.</param>
        /// <returns>An emotion record with the ID or null if it doesn't exist.</returns>
        public static EmotionRecord GetEmotionRecord(int id)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {emotionTableName} WHERE id = @id");
            cmd.Bind("@id", id);

            var records = cmd.ExecuteQuery<EmotionRecord>();

            if (records.Count > 0)
            {
                return records.First();
            }

            return null;
        }

        /// <summary>
        /// Gets an emotion record.
        /// </summary>
        /// <param name="emotionName">The emotion name.</param>
        /// <returns>An emotion record with the ID or null if it doesn't exist.</returns>
        public static EmotionRecord GetEmotionRecord(string emotionName)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {emotionTableName} WHERE LOWER(emotion_name) = LOWER(@emotion_name)");
            cmd.Bind("@emotion_name", emotionName);

            var records = cmd.ExecuteQuery<EmotionRecord>();

            if (records.Count > 0)
            {
                return records.First();
            }

            return null;
        }

        /// <summary>
        /// Gets all emotion records within a range and only includes those that contain a given string.
        /// </summary>
        /// <param name="min">The min range.</param>
        /// <param name="max">The max range.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="desc">If descending order by mode should be used.</param>
        /// <returns>All emotion records within range filtered by the pattern.</returns>
        public static List<EmotionRecord> GetEmotionRecords(int min, int max, string pattern, bool desc)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {emotionTableName} WHERE LOWER(emotion_name) LIKE LOWER(@pattern) ORDER BY LOWER(emotion_name) {(desc ? "DESC" : "ASC")} LIMIT @max OFFSET @min");
            cmd.Bind("@min", min);
            cmd.Bind("@max", max);
            cmd.Bind("@pattern", $"%{pattern}%");

            var records = cmd.ExecuteQuery<EmotionRecord>();
            return records.ToList();
        }

        /// <summary>
        /// Gets all emotion records that contain a given string.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="desc">If descending order by mode should be used.</param>
        /// <returns>All emotion records within range filtered by the pattern.</returns>
        public static List<EmotionRecord> GetEmotionRecords(string pattern, bool desc)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {emotionTableName} WHERE LOWER(emotion_name) LIKE LOWER(@pattern) ORDER BY LOWER(emotion_name) {(desc ? "DESC" : "ASC")}");
            cmd.Bind("@pattern", $"%{pattern}%");

            var records = cmd.ExecuteQuery<EmotionRecord>();
            return records.ToList();
        }

        /// <summary>
        /// Gets the total number of records in the emotion table.
        /// </summary>
        /// <returns>The total number of records.</returns>
        public static int GetEmotionCount()
        {
            return GetRecordCount(emotionTableName);
        }

        /// <summary>
        /// Gets the total number of records in the emotion table filtered by the pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>The total number of filtered records.</returns>
        public static int GetFilteredEmotionCount(string pattern)
        {
            int count = connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM {emotionTableName} WHERE LOWER(emotion_name) LIKE LOWER(?)", $"{pattern}");
            return count;
        }
    }
}