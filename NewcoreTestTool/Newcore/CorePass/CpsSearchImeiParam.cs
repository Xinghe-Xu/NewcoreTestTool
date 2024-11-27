namespace NewcoreTestTool
{
    public class CpsSearchImeiParam
    {

        public Body body { get; set; }
        public class Body
        {
            public string templateApiName { get; set; }
            public int start { get; set; }
            public bool fuzzy { get; set; }
            public int length { get; set; }
            public Workflow workflow { get; set; }
            public Defaultsort defaultSort { get; set; }
            public Conditionalfilter conditionalFilter { get; set; }
        }

        public class Workflow
        {
            public string tab { get; set; }
            public int staffId { get; set; }
        }

        public class Defaultsort
        {
            public bool enabled { get; set; }
            public string view { get; set; }
        }

        public class Conditionalfilter
        {
            public Conditiondefinition conditionDefinition { get; set; }
        }

        public class Conditiondefinition
        {
            public Node[] nodes { get; set; }
        }

        public class Node
        {
            public string dataSourceType { get; set; }
            public string leftFieldTemplateApiName { get; set; }
            public string leftFieldApiName { get; set; }
            public string conditionType { get; set; }
            public string rightConstant { get; set; }
            public string leftParentheses { get; set; }
            public string rightParentheses { get; set; }
        }

        public static CpsSearchImeiParam MakeTestCpsParam(string SN)
        {
            return new CpsSearchImeiParam()
            {
                body = new Body()
                {
                    templateApiName = "SN_IMEI",
                    start = 0,
                    fuzzy = true,
                    length = 100,
                    workflow = new Workflow()
                    {
                        tab = "ALL",
                        staffId = 10043802
                    },
                    defaultSort = new Defaultsort()
                    {
                        enabled = true,
                        view = "ALL"
                    },
                    conditionalFilter = new Conditionalfilter()
                    {
                        conditionDefinition = new Conditiondefinition()
                        {
                            nodes = new Node[]
                            {
                                new Node()
                                {
                                    dataSourceType="VARIABLE",
                                    leftFieldTemplateApiName="SN_IMEI",
                                    leftFieldApiName="SN",
                                    conditionType="EQ",
                                    leftParentheses="(",
                                    rightParentheses=")",
                                    rightConstant=SN
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
