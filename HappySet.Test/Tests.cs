using System.Linq;
using NUnit.Framework;

namespace HappySet.Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Logから取得するIdが正しいこと()
        {
            var set = new HappySet<TestClass>();

            for (var i = 0; i < 5; i++)
            {
                var id = set.Commit($"commit{i}");
                var last = set.Logs().Last();
                Assert.AreEqual(id, last.Id);
            }
        }

        [Test]
        public void MaxHistoryが正常に機能すること()
        {
            var set = new HappySet<TestClass>();

            for (var i = 0; i < 10; i++)
            {
                var id = set.Commit($"commit{i}");
                var last = set.Logs().Last();
                if (i > 4)
                {
                    Assert.AreEqual(5, set.CommitCount);
                    Assert.AreNotEqual(id, last);
                    continue;
                }
                Assert.AreEqual(id, last.Id);
            }

            set = new HappySet<TestClass>(10);

            for (var i = 0; i < 10; i++)
            {
                var id = set.Commit($"commit{i}");
                var last = set.Logs().Last();
                Assert.AreEqual(id, last.Id);
            }
        }

        [Test]
        public void Commitが正常に機能すること()
        {
            var set = new HappySet<TestClass>();

            Assert.AreEqual(0, set.CommitCount);

            var aid = set.Commit("a");

            Assert.AreEqual(1, set.CommitCount);
            Assert.AreEqual("a", set.Log(aid).Message);

            var bid = set.Commit("b");

            Assert.AreEqual(2, set.CommitCount);
            Assert.AreEqual("b", set.Log(bid).Message);

            var cid = set.Commit("c");

            Assert.AreEqual(3, set.CommitCount);
            Assert.AreEqual("c", set.Log(cid).Message);
        }

        [Test]
        public void Checkoutにより中身が切り替わること()
        {
            var set = new HappySet<TestClass>(10)
            {
                new TestClass("a")
            };

            var aid = set.Commit("a");

            set.Add(new TestClass("b"));
            var bid = set.Commit("b");

            Assert.AreEqual(2, set.Count);
            Assert.AreEqual(2, set.CommitCount);
            Assert.AreEqual(2, set.Logs().Count);

            set.Checkout(aid);

            Assert.AreEqual(1, set.Count);
            Assert.AreEqual(2, set.CommitCount);
            Assert.AreEqual(2, set.Logs().Count);

            set.Checkout(bid);

            Assert.AreEqual(2, set.Count);
            Assert.AreEqual(2, set.CommitCount);
            Assert.AreEqual(2, set.Logs().Count);

            set.Add(new TestClass("c"));
            set.Commit("c");

            set.Add(new TestClass("d"));
            var did = set.Commit("d");

            Assert.AreEqual(4, set.Count);
            Assert.AreEqual(4, set.CommitCount);
            Assert.AreEqual(4, set.Logs().Count);

            set.Checkout(bid);

            Assert.AreEqual(2, set.Count);
            Assert.AreEqual(4, set.CommitCount);
            Assert.AreEqual(4, set.Logs().Count);

            set.Checkout(did);

            Assert.AreEqual(4, set.Count);
            Assert.AreEqual(4, set.CommitCount);
            Assert.AreEqual(4, set.Logs().Count);

            set.Checkout(did);

            Assert.AreEqual(4, set.Count);
            Assert.AreEqual(4, set.CommitCount);
            Assert.AreEqual(4, set.Logs().Count);

            set.Checkout(aid);

            set.Add(new TestClass("e"));
            var eid = set.Commit("e");

            Assert.AreEqual(2, set.Count);
            Assert.AreEqual(5, set.CommitCount);
            Assert.AreEqual(5, set.Logs().Count);

            var a = set.ElementAt(0);
            set.Remove(a);
            set.Commit("f");

            Assert.AreEqual(1, set.Count);
            Assert.AreEqual(6, set.CommitCount);
            Assert.AreEqual(6, set.Logs().Count);

            set.Checkout(eid);

            Assert.AreEqual(2, set.Count);
            Assert.AreEqual(6, set.CommitCount);
            Assert.AreEqual(6, set.Logs().Count);
        }

        [Test]
        public void CherryPickによって特定のSetを取得できること()
        {
            var set = new HappySet<TestClass>();

            set.Commit("a");

            Assert.AreEqual(0, set.Count);
            Assert.AreEqual(1, set.CommitCount);
            Assert.AreEqual(1, set.Logs().Count);

            set.Add(new TestClass("b"));
            var bid = set.Commit("b");

            set.Add(new TestClass("c"));
            var cid = set.Commit("c");

            var bset = set.CherryPick(bid);

            Assert.AreEqual(1, bset.Count);

            var cset = set.CherryPick(cid);

            Assert.AreEqual(2, cset.Count);

            set.Add(new TestClass("d"));

            Assert.AreEqual(2, cset.Count);

            Assert.AreEqual(3, set.Count);
            Assert.AreEqual(3, set.CommitCount);
            Assert.AreEqual(3, set.Logs().Count);
        }

        [Test]
        public void Resetで特定の履歴までクリアされること()
        {
            var set = new HappySet<TestClass>
            {
                new TestClass("a")
            };

            set.Commit("a");

            set.Add(new TestClass("b"));

            var bid = set.Commit("b");

            set.Add(new TestClass("c"));
            set.Commit("c");

            set.Add(new TestClass("d"));
            set.Commit("d");

            Assert.AreEqual(4, set.Count);
            Assert.AreEqual(4, set.CommitCount);
            Assert.AreEqual(4, set.Logs().Count);

            set.Reset(bid);

            Assert.AreEqual(2, set.Count);
            Assert.AreEqual(2, set.CommitCount);
            Assert.AreEqual(2, set.Logs().Count);

            set.Add(new TestClass("e"));
            set.Commit("e");

            Assert.AreEqual(3, set.Count);
            Assert.AreEqual(3, set.CommitCount);
            Assert.AreEqual(3, set.Logs().Count);
        }

        [Test]
        public void HappySetの中身が正しいこと()
        {
            var set = new HappySet<TestClass>
            {
                new TestClass("1"),
                new TestClass("2")
            };

            Assert.AreEqual("1", set.ElementAt(0).Hoge);
            Assert.AreEqual("2", set.ElementAt(1).Hoge);

            set.Commit("a");
            set.Add(new TestClass("3"));
            set.Commit("b");

            Assert.AreEqual("1", set.ElementAt(0).Hoge);
            Assert.AreEqual("2", set.ElementAt(1).Hoge);
            Assert.AreEqual("3", set.ElementAt(2).Hoge);

            Assert.AreEqual("a", set.Logs()[0].Message);

            var id = set.Logs()[0].Id;

            set.Checkout(id);
            Assert.AreEqual(2, set.CommitCount);

            Assert.AreEqual("1", set.ElementAt(0).Hoge);
            Assert.AreEqual("2", set.ElementAt(1).Hoge);

            var set2 = set.CherryPick(id);

            Assert.AreEqual("1", set2.ElementAt(0).Hoge);
            Assert.AreEqual("2", set2.ElementAt(1).Hoge);

            set2.Add(new TestClass("3"));

            Assert.AreEqual("1", set2.ElementAt(0).Hoge);
            Assert.AreEqual("2", set2.ElementAt(1).Hoge);
            Assert.AreEqual("3", set2.ElementAt(2).Hoge);

            Assert.AreEqual("1", set.ElementAt(0).Hoge);
            Assert.AreEqual("2", set.ElementAt(1).Hoge);

            id = set.Logs()[1].Id;

            set.Checkout(id);
            Assert.AreEqual(2, set.CommitCount);

            Assert.AreEqual("1", set.ElementAt(0).Hoge);
            Assert.AreEqual("2", set.ElementAt(1).Hoge);
            Assert.AreEqual("3", set.ElementAt(2).Hoge);

            set = new HappySet<TestClass>
            {
                new TestClass("a"),
                new TestClass("b")
            };

            Assert.AreEqual("a", set.ElementAt(0).Hoge);
            Assert.AreEqual("b", set.ElementAt(1).Hoge);
        }

        [Test]
        public void Extensionが正常に動作すること()
        {
            var set = new HappySet<TestClass>();
            var first = set.FirstOrDefault(new TestClass("hoge"));

            Assert.AreEqual("hoge", first.Hoge);

            first = set.FirstOrDefault(() => new TestClass("huge"));

            Assert.AreEqual("huge", first.Hoge);

            set.Add(new TestClass("a"));

            first = set.FirstOrDefault(new TestClass("hoge"));

            Assert.AreEqual("a", first.Hoge);

            first = set.FirstOrDefault(() => new TestClass("huge"));

            Assert.AreEqual("a", first.Hoge);
        }
    }
}