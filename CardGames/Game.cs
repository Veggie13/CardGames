using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Linq;

namespace CardGames
{
    public class Game
    {
        private Game() { }

        private static Dictionary<string, LayoutElement.IFactory> ElementFactories = new Dictionary<string, LayoutElement.IFactory>();

        static Game()
        {
            StyleParser["fixed"] = TableLayoutElement.StyleType.Fixed;
            StyleParser["grow"] = TableLayoutElement.StyleType.Grow;
            StyleParser["auto"] = TableLayoutElement.StyleType.AutoSize;

            ElementFactories["card"] = new SingleCardElement.Factory();
            ElementFactories["stack"] = new CardStackElement.Factory();
        }

        public abstract class LayoutElement
        {
            public interface IFactory
            {
                LayoutElement Create(TableLayoutElement parent, string name);
            }

            private static Dictionary<string, LayoutElement> Elements = new Dictionary<string, LayoutElement>();

            public static T Get<T>(string name) where T : LayoutElement
            {
                return Elements[name] as T;
            }

            internal LayoutElement(TableLayoutElement parent)
            {
                Parent = parent;
            }
            
            public LayoutElement(TableLayoutElement parent, string name)
            {
                Parent = parent;
                Name = name;
                Elements[name] = this;
            }

            public string Name { get; set; }
            public abstract float MinWidth { get; }
            public abstract float MinHeight { get; }

            public bool AllowActivation { get; set; }
            public bool AllowGrabbing { get; set; }
            public bool AllowDrop { get; set; }
            
            public TableLayoutElement Parent { get; private set; }

            public virtual float BasicWidth { get { return Parent.BasicWidth; } }
            public virtual float BasicHeight { get { return Parent.BasicHeight; } }

            public IEnumerable<Card> BeginGrab()
            {
                return AllowGrabbing ? beginGrab() : null;
            }

            protected virtual Card beginGrab()
            {
                return null;
            }

            public virtual void FinishGrab(bool complete)
            {
            }

            public bool Drop(IEnumerable<Card> cards)
            {
                return AllowDrop ? drop(cards) : false;
            }

            protected virtual bool drop(IEnumerable<Card> cards)
            {
                return false;
            }

            public virtual Card Draw()
            {
                return null;
            }

            public void Activate()
            {
                if (AllowActivation)
                    activate();
            }
            
            protected virtual void activate() { }

            public virtual void Render(double originX, double originY, double width, double height, ICardRenderer renderer)
            {
            }

            public virtual LayoutElement DeepestElementAt(double x, double y)
            {
                return this;
            }

            public event Action Modified;
            
            protected void clearAttachedElements()
            {
                Modified = null;
            }

            protected void emitModified()
            {
                if (Modified != null)
                    Modified();
            }

            internal virtual void attachElements() { }
        }

        public class NullElement : LayoutElement
        {
            public NullElement(TableLayoutElement parent) : base(parent) { }

            public override float MinWidth
            {
                get { return 0f; }
            }

            public override float MinHeight
            {
                get { return 0f; }
            }
        }

        public abstract class CardElement : LayoutElement
        {
            public CardElement(TableLayoutElement parent, string name) : base(parent, name) { }

            public override float MinWidth
            {
                get { return BasicWidth; }
            }

            public override float MinHeight
            {
                get { return BasicHeight; }
            }
        }

        public class SingleCardElement : CardElement
        {
            public class Factory : IFactory
            {
                public LayoutElement Create(TableLayoutElement parent, string name)
                {
                    return new SingleCardElement(parent, name);
                }
            }

            private Card _card = null;

            public SingleCardElement(TableLayoutElement parent, string name) : base(parent, name) { }
        }

        public class CardStackElement : CardElement
        {
            public class Factory : IFactory
            {
                public LayoutElement Create(TableLayoutElement parent, string name)
                {
                    return new CardStackElement(parent, name);
                }
            }

            public CardStack CardStack { get; private set; }
            
            public CardStackElement(TableLayoutElement parent, string name) : base(parent, name)
            {
                CardStack = new CardStack(name);
                CardStack.Modified += new Action(emitModified);
            }

            private Card _grabbed = null;
            private CardVisibility _grabbedVis = CardVisibility.FaceDown;
            
            protected override Card beginGrab()
            {
                _grabbedVis = CardStack.Top != null ? CardStack.Top.Visibility : _grabbedVis;
                _grabbed = Draw();
                if (_grabbed != null)
                {
                    _grabbed.Grab();
                }
                return _grabbed;
            }

            public override void FinishGrab(bool complete)
            {
                if (_grabbed != null && !complete)
                {
                    _grabbed.Visibility = _grabbedVis;
                    _grabbed.StackOnto(CardStack);
                    _grabbed = null;
                }
            }

            protected override bool drop(IEnumerable<Card> cards)
            {
                return cards.StackOnto(CardStack);
            }

            public override Card Draw()
            {
                List<Card> drawn = new List<Card>();
                if (CardStack.DrawFromTop(drawn, 1) < 1)
                    return null;
                return drawn[0];
            }

            protected override void activate()
            {
                CardStack.Activate();
            }

            public override void Render(double originX, double originY, double width, double height, ICardRenderer renderer)
            {
                renderer.Render(originX, originY, 0, 0, CardStack);
            }
        }

        public class FanStackElement : CardElement
        {
            public CardStack CardStack { get; private set; }

            public FanStackElement(TableLayoutElement parent, string name) : base(parent, name)
            {
                CardStack = new CardStack(name);
                CardStack.Modified += new Action(emitModified);

                _grabbed = new CardStack(name + "_grabbed");
            }

            private CardStack _grabbed;

            
        }

        public class LayoutElementTable : IEnumerable<LayoutElement>
        {
            internal LayoutElementTable(EventHandler handler)
            {
                SizeChanged += handler;
            }

            public event EventHandler SizeChanged;

            private LayoutElement[,] _table = new LayoutElement[1, 1];
            public LayoutElement this[int row, int col]
            {
                get
                {
                    return _table[row, col];
                }
                set
                {
                    if (_table.GetLength(0) <= row || _table.GetLength(1) <= col)
                    {
                        int newRowCount = Math.Max(_table.GetLength(0), row + 1);
                        int newColCount = Math.Max(_table.GetLength(1), col + 1);

                        var newTable = new LayoutElement[newRowCount, newColCount];

                        for (int i = 0; i < _table.GetLength(0); i++)
                            for (int j = 0; j < _table.GetLength(1); j++)
                                newTable[i, j] = _table[i, j];

                        _table = newTable;

                        SizeChanged(this, new EventArgs());
                    }

                    _table[row, col] = value;
                }
            }

            public int RowCount { get { return _table.GetLength(0); } }
            public int ColCount { get { return _table.GetLength(1); } }

            public float MinColWidth(int col)
            {
                return _table.IterateOverCol(col).Select((e) => (e != null ? e.MinWidth : 0f)).Max();
            }

            public float MinWidth
            {
                get
                {
                    return _table.Cols().Select((c) => MinColWidth(c)).Sum();
                }
            }

            public float MinRowHeight(int row)
            {
                return _table.IterateOverRow(row).Select((e) => (e != null ? e.MinHeight : 0f)).Max();
            }

            public float MinHeight
            {
                get
                {
                    return _table.Rows().Select((r) => MinRowHeight(r)).Sum();
                }
            }
        
            public IEnumerator<LayoutElement> GetEnumerator()
            {
 	            foreach (var element in _table)
                    yield return element;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
 	            return GetEnumerator();
            }
        }

        public class TableLayoutElement : LayoutElement
        {
            public enum StyleType
            {
                Fixed,
                Grow,
                AutoSize
            }

            public struct Style
            {
                public StyleType Type;
                public float Value;
            }

            public TableLayoutElement(TableLayoutElement parent, string name) : base(parent, name)
            {
                ColumnStyles = new Style[1];
                RowStyles = new Style[1];

                Table = new LayoutElementTable((o, ea) =>
                {
                    if (ColumnStyles.Length != Table.ColCount)
                    {
                        var newColStyles = new Style[Table.ColCount];
                        ColumnStyles.CopyTo(newColStyles, 0);
                        ColumnStyles = newColStyles;
                    }
                    if (RowStyles.Length != Table.RowCount)
                    {
                        var newRowStyles = new Style[Table.RowCount];
                        RowStyles.CopyTo(newRowStyles, 0);
                        RowStyles = newRowStyles;
                    }
                });
            }

            public LayoutElementTable Table { get; private set; }

            public Style[] ColumnStyles { get; private set; }

            public Style[] RowStyles { get; private set; }

            private float _basicWidth = 0.1f;
            public float BasicWidth
            {
                get
                {
                    if (Parent == null)
                        return _basicWidth;
                    return base.BasicWidth;
                }
                set
                {
                    _basicWidth = value;
                }
            }

            private float _basicHeight = 0.1f;
            public float BasicHeight
            {
                get
                {
                    if (Parent == null)
                        return _basicHeight;
                    return base.BasicHeight;
                }
                set
                {
                    _basicHeight = value;
                }
            }
            
            public override float MinWidth
            {
                get { return Table.MinWidth; }
            }

            public override float MinHeight
            {
                get { return Table.MinHeight; }
            }

            public override void Render(double originX, double originY, double width, double height, ICardRenderer renderer)
            {
                double x1 = originX - width / 2;
                double y1 = originY - height / 2;

                double totalFixedCols = ColumnStyles.Where((s) => (s.Type == StyleType.Fixed)).Select((s) => s.Value).Sum();
                double totalAutoCols = ColumnStyles.Select((s, i) => i).Where((i) => ColumnStyles[i].Type == StyleType.AutoSize).Select((i) => Table.MinColWidth(i)).Sum();
                double totalColGrowSpace = width - totalFixedCols - totalAutoCols;
                double totalColGrowValue = ColumnStyles.Where((s) => (s.Type == StyleType.Grow)).Select((s) => s.Value).Sum();

                double totalFixedRows = RowStyles.Where((s) => (s.Type == StyleType.Fixed)).Select((s) => s.Value).Sum();
                double totalAutoRows = RowStyles.Select((s, i) => i).Where((i) => RowStyles[i].Type == StyleType.AutoSize).Select((i) => Table.MinRowHeight(i)).Sum();
                double totalRowGrowSpace = height - totalFixedRows - totalAutoRows;
                double totalRowGrowValue = RowStyles.Where((s) => (s.Type == StyleType.Grow)).Select((s) => s.Value).Sum();

                double y = y1;
                for (int row = 0; row < Table.RowCount; row++)
                {
                    double rowHeight = 0;
                    switch (RowStyles[row].Type)
                    {
                        case StyleType.Fixed:
                            rowHeight = RowStyles[row].Value;
                            break;
                        case StyleType.AutoSize:
                            rowHeight = Table.MinRowHeight(row);
                            break;
                        case StyleType.Grow:
                            rowHeight = totalRowGrowSpace * RowStyles[row].Value / totalRowGrowValue;
                            break;
                    }

                    double x = x1;
                    for (int col = 0; col < Table.ColCount; col++)
                    {
                        double colWidth = 0;
                        switch (ColumnStyles[col].Type)
                        {
                            case StyleType.Fixed:
                                colWidth = ColumnStyles[col].Value;
                                break;
                            case StyleType.AutoSize:
                                colWidth = Table.MinColWidth(col);
                                break;
                            case StyleType.Grow:
                                colWidth = totalColGrowSpace * ColumnStyles[col].Value / totalColGrowValue;
                                break;
                        }

                        if (Table[row, col] != null)
                            Table[row, col].Render(x + colWidth / 2, y + rowHeight / 2, colWidth, rowHeight, renderer);

                        x += colWidth;
                    }

                    y += rowHeight;
                }
            }

            public override LayoutElement DeepestElementAt(double x, double y)
            {
                int row = 0, col = 0;
                double x1 = 0, y1 = 0, x2 = 0, y2 = 0;

                double totalFixedRows = RowStyles.Where((s) => (s.Type == StyleType.Fixed)).Select((s) => s.Value).Sum();
                double totalAutoRows = RowStyles.Select((s, i) => i).Where((i) => RowStyles[i].Type == StyleType.AutoSize).Select((i) => Table.MinRowHeight(i)).Sum();
                double totalRowGrowSpace = 1 - totalFixedRows - totalAutoRows;
                double totalRowGrowValue = RowStyles.Where((s) => (s.Type == StyleType.Grow)).Select((s) => s.Value).Sum();

                double rowHeight = 0;
                for (; row < RowStyles.Length; row++)
                {
                    switch (RowStyles[row].Type)
                    {
                        case StyleType.Fixed:
                            rowHeight = RowStyles[row].Value;
                            break;
                        case StyleType.AutoSize:
                            rowHeight = Table.MinRowHeight(row);
                            break;
                        case StyleType.Grow:
                            rowHeight = totalRowGrowSpace * RowStyles[row].Value / totalRowGrowValue;
                            break;
                    }

                    y1 = y2;
                    y2 += rowHeight;
                    if (y2 >= y)
                        break;
                }

                double totalFixedCols = ColumnStyles.Where((s) => (s.Type == StyleType.Fixed)).Select((s) => s.Value).Sum();
                double totalAutoCols = ColumnStyles.Select((s, i) => i).Where((i) => ColumnStyles[i].Type == StyleType.AutoSize).Select((i) => Table.MinColWidth(i)).Sum();
                double totalColGrowSpace = 1 - totalFixedCols - totalAutoCols;
                double totalColGrowValue = ColumnStyles.Where((s) => (s.Type == StyleType.Grow)).Select((s) => s.Value).Sum();

                double colWidth = 0;
                for (; col < ColumnStyles.Length; col++)
                {
                    switch (ColumnStyles[col].Type)
                    {
                        case StyleType.Fixed:
                            colWidth = ColumnStyles[col].Value;
                            break;
                        case StyleType.AutoSize:
                            colWidth = Table.MinColWidth(col);
                            break;
                        case StyleType.Grow:
                            colWidth = totalColGrowSpace * ColumnStyles[col].Value / totalColGrowValue;
                            break;
                    }

                    x1 = x2;
                    x2 += colWidth;
                    if (x2 >= x)
                        break;
                }

                if (row >= Table.RowCount || col >= Table.ColCount)
                    return this;
                if (Table[row, col] == null)
                    return this;

                double nextX = x - x1;
                double nextY = y - y1;

                return Table[row, col].DeepestElementAt(nextX, nextY);
            }

            internal override void attachElements()
            {
                clearAttachedElements();

 	            foreach (var element in Table)
                {
                    if (element != null)
                    {
                        element.attachElements();
                        element.Modified += new Action(emitModified);
                    }
                }
            }
        }

        public static Game ParseDefinition(string xmlDefn)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlDefn);

            var roots = doc.GetElementsByTagName("cardgame");
            if (roots.Count < 1)
                return null;
            var root = roots[0];

            var game = new Game();
            game.Name = root.Attributes["name"].Value;

            game.TableLayout = parseTableElement(null, root);

            return game;
        }

        private static LayoutElement parseElement(TableLayoutElement parent, XmlNode element)
        {
            string elementName = element.Attributes["name"] != null ? element.Attributes["name"].Value : "";

            if (!ElementFactories.ContainsKey(element.Name))
                return new NullElement(parent);
            if (element.Name.Equals("table"))
                return parseTableElement(parent, element);

            return ElementFactories[element.Name].Create(parent, elementName);
        }

        private static TableLayoutElement parseTableElement(TableLayoutElement parent, XmlNode element)
        {
            string elementName = element.Attributes["name"].Value;
            var table = new TableLayoutElement(parent, elementName);

            var layout = element.FirstChild;
            if (!layout.Name.Equals("layout"))
                return null;

            var node = layout.FirstChild;
            int row = 0;
            while (node != null)
            {
                if (node.Name.Equals("row"))
                    parseRow(table, row++, node);

                node = node.NextSibling;
            }

            var styles = layout.NextSibling;
            if (!styles.Name.Equals("styles"))
                return null;

            node = styles.FirstChild;
            int col = row = 0;
            while (node != null)
            {
                if (node.Name.Equals("row"))
                {
                    table.RowStyles[row++] = parseStyle(node);
                }
                else if (node.Name.Equals("col"))
                {
                    table.ColumnStyles[col++] = parseStyle(node);
                }

                node = node.NextSibling;
            }

            return table;
        }

        private static void parseRow(TableLayoutElement table, int row, XmlNode rowNode)
        {
            var node = rowNode.FirstChild;
            int col = 0;
            while (node != null)
            {
                var child = parseElement(table, node);

                if (node.Attributes["allowGrabbing"] != null)
                    child.AllowGrabbing = node.Attributes["allowGrabbing"].Value.Equals("true");
                if (node.Attributes["allowActivation"] != null)
                    child.AllowActivation = node.Attributes["allowActivation"].Value.Equals("true");
                if (node.Attributes["allowDrop"] != null)
                    child.AllowDrop = node.Attributes["allowDrop"].Value.Equals("true");

                table.Table[row, col++] = child;

                node = node.NextSibling;
            }
        }

        private static Dictionary<string, TableLayoutElement.StyleType> StyleParser = new Dictionary<string, TableLayoutElement.StyleType>();
        private static TableLayoutElement.Style parseStyle(XmlNode node)
        {
            return new TableLayoutElement.Style() { Type = StyleParser[node.Attributes["type"].Value], Value = float.Parse(node.Attributes["value"].Value) };
        }

        public string Name { get; private set; }

        public event Action Modified;
        
        private TableLayoutElement _tableLayout;
        public TableLayoutElement TableLayout
        {
            get { return _tableLayout; }
            private set
            {
                if (_tableLayout != null)
                {
                    Modified = null;
                }

                _tableLayout = value;

                if (_tableLayout != null)
                {
                    _tableLayout.attachElements();
                    _tableLayout.Modified += new Action(emitModified);
                }
            }
        }

        private void emitModified()
        {
            if (Modified != null)
                Modified();
        }
    }

    static partial class Extensions
    {
        public static IEnumerable<T> IterateOverRow<T>(this T[,] arr, int row)
        {
            foreach (int col in arr.Cols())
                yield return arr[row, col];
        }

        public static IEnumerable<T> IterateOverCol<T>(this T[,] arr, int col)
        {
            foreach (int row in arr.Rows())
                yield return arr[row, col];
        }

        public static IEnumerable<int> Rows<T>(this T[,] arr)
        {
            for (int row = 0; row < arr.GetLength(0); row++)
                yield return row;
        }

        public static IEnumerable<int> Cols<T>(this T[,] arr)
        {
            for (int col = 0; col < arr.GetLength(1); col++)
                yield return col;
        }
    }
}
