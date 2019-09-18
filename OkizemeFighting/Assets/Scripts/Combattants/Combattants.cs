namespace SA
{
    public static class StaticClass
    {
        public static CardInstance Attacker { get; set; }
        public static CardInstance Blocker { get; set; }
        public static PlayerHolder Player { get; set; }
        public static PlayerHolder Ennemy { get; set; }
        public static int AttackValue { get; set; }
        public static string AtkName { get; set; }
        public static string DefName { get; set; }
    }
}