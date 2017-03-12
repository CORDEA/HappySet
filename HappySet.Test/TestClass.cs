namespace HappySet.Test
{
    public class TestClass : IMHappy
    {
        public TestClass(string hoge)
        {
            Hoge = hoge;
        }

        public string Hoge { get; }

        public IMHappy Clone()
        {
            return new TestClass(Hoge);
        }
    }
}