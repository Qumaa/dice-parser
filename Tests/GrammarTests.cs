namespace Tests
{
    [TestClass]
    public class GrammarTests
    {
        [TestMethod]
        public void Example1()
        {
            GrammarGraph graph = new GrammarGraphBuilder()
                .Add("uscalar", Grammar.UnsignedInteger())
                .Add("scalar", Grammar.AsOptional(Grammar.Literal("-")).Then(Grammar.Reference("uscalar")))
                .Add(
                    "die",
                    Grammar.AsOptional(Grammar.Reference("uscalar"), defaultValue: "1")
                        .Then(Grammar.Literal("d").Then(Grammar.Reference("uscalar")))
                    )
                .Add("bool", Grammar.Literal(DiceRoll.Syntax.Params("true", "false"), StringComparison.OrdinalIgnoreCase))
                .Add("add", Grammar.Reference("int").Then(Grammar.Literal("+")).Then(Grammar.Reference("int")))
                .Add("subtract", Grammar.Reference("int").Then(Grammar.Literal("-")).Then(Grammar.Reference("int")))
                .Add("multiply", Grammar.Reference("int").Then(Grammar.Literal("*")).Then(Grammar.Reference("int")))
                .Add("divide", Grammar.Reference("int").Then(Grammar.Literal("/")).Then(Grammar.Reference("int")))
                .Add(
                    "int",
                    Grammar.Reference(
                        "uscalar",
                        "scalar",
                        "die"
                        )
                    )
                .Build();

            CompletedChain[] chains = graph.Parse("4 * 5 + 6");

            foreach (CompletedChain chain in chains)
                Console.WriteLine(chain.Tag);
        }
    }
}
