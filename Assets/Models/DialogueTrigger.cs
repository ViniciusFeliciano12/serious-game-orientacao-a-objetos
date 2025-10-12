namespace Assets.Models
{
    public enum SkillDialogueTrigger
    {
        BeforeLearningSkill,
        AfterLearningSkill,
        BeforeCreatingSkill,
        AfterCreatingSkill,
        BeforeEditSkill,
        AfterEditSkill
    }

    [System.Serializable]
    public class DialogueTrigger
    {
        public SkillDialogueTrigger WhenTrigger;
        public string DialogueName;
    }
}
