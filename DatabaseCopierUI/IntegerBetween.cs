namespace DatabaseCopierUI
{
    public static class IntegerBetween
    {
        public static bool Between(this int num, int lower, int upper)
        {
            return (num >= lower && num <= upper);
        }
    }
}