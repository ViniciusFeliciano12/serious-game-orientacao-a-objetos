// Este script DEVE estar em uma pasta chamada "Editor"
using UnityEditor;
using UnityEngine;

public class SaveEditorTools
{
    // O nome do menu agora reflete que ele apaga TUDO.
    [MenuItem("Ferramentas/Apagar TODOS os Saves (Editor e Build)")]
    public static void DeleteAllSaves()
    {
        // Exibe uma caixa de diálogo de confirmação ANTES de apagar.
        if (EditorUtility.DisplayDialog(
            "Apagar Todos os Saves",
            "Você tem certeza que deseja apagar o save do Editor E o save da Build?\n\nO save da build pode conter progresso de jogo real e esta ação não pode ser desfeita.",
            "Sim, apagar tudo", "Cancelar"))
        {
            // Se o usuário confirmar, chama os dois métodos de exclusão.
            SaveSystem.DeleteEditorSave();
            SaveSystem.DeleteBuildSave();

            // Mostra uma janela de confirmação final para o desenvolvedor.
            EditorUtility.DisplayDialog("Limpeza de Save", "O arquivo de save do editor e da build foram apagados com sucesso!", "Ok");
        }
    }
}