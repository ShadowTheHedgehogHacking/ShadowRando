using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShadowRando.Core
{
    public class MarkovTextModel
    {
        internal class MarkovNode
        {
            public string Ch;
            public int Count;
            public int FollowCount;

            public Dictionary<string, MarkovNode> Children;
            public MarkovNode(string c)
            {
                Ch = c;
                Count = 1;
                FollowCount = 0;
                Children = new Dictionary<string, MarkovNode>();
            }

            public MarkovNode AddChild(string c)
            {
                if (Children == null)
                    Children = new Dictionary<string, MarkovNode>();
                FollowCount += 1;
                MarkovNode child = null;
                if (Children.TryGetValue(c, out child))
                {
                    child.Count += 1;
                }
                else
                {
                    child = new MarkovNode(c);
                    Children.Add(c, child);
                }
                return child;
            }
        }
        public const string StartChar = "\ufffe";

        public const string StopChar = "\uffff";
        private MarkovNode Root = new MarkovNode(StartChar);

        private readonly int ModelOrder;
        public MarkovTextModel(int order)
        {
            ModelOrder = order;
        }

        public void AddString(string s)
        {
            // Construct the string that will be added.
            List<string> arr = new List<string>(Enumerable.Repeat(StartChar, ModelOrder));
            arr.AddRange(s.Split(' '));
			arr.AddRange(Enumerable.Repeat(StopChar, ModelOrder));
            // Naive method
            for (int iStart = 0; iStart < arr.Count; iStart++)
            {
                // Get the order 0 node
                MarkovNode parent = Root.AddChild(arr[iStart]);

                // Now add N-grams starting with this node
                int i = 1;
                while (i <= ModelOrder && i + iStart < arr.Count)
                {
                    MarkovNode child = parent.AddChild(arr[iStart + i]);
                    parent = child;
                    i += 1;
                }
            }
        }

        public void AddStrings(params string[] s)
        {
            foreach (string item in s)
                AddString(item);
        }

        public void Clear()
        {
            Root = new MarkovNode(StartChar);
        }

        public string Generate(Random r)
        {
            List<string> rslt = new List<string>();
            for (int i = 0; i < ModelOrder; i++)
                rslt.Add(StartChar);
            int iStart = 0;
            string ch;
            do
            {
                MarkovNode node = Root.Children[rslt[iStart]];
                for (int i = 1; i < ModelOrder; i++)
                    node = node.Children[rslt[i + iStart]];
                ch = SelectChildChar(r, node);
                if (ch != StopChar.ToString())
                    rslt.Add(ch);
                iStart += 1;
            } while (ch != StopChar.ToString());

			return string.Join(' ', rslt.Skip(ModelOrder));
        }

        private string SelectChildChar(Random r, MarkovNode node)
        {
            // Generate a random number in the range 0..(node.Count-1)
            int rnd = r.Next(node.FollowCount);

            // Go through the children to select the node
            int cnt = 0;
            foreach (KeyValuePair<string, MarkovNode> kvp in node.Children)
            {
                cnt += kvp.Value.Count;
                if (cnt > rnd)
                {
                    return kvp.Key;
                }
            }
            throw new ApplicationException("This can't happen!");
        }
    }
}