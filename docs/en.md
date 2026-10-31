# Expression Engine

Use numeric expressions in any game, including a visual novel with no RPG or stats modules. Dialog Engine and Schemas depend on this library; Stats Engine reuses it for character formulas.

```scenario
set money=rnd(3,5)
cset total "1+2+${money}"
ptext "You found ${money} coins."
action item.add id=money/coin count=${money}
```

`rnd(3,5)` chooses an integer from **3 through 5 inclusive**, once when the assignment executes. Both later lines read that stored value. The inventory action requires Items Engine and a game that defines `money/coin`; the arithmetic and text need no RPG. A localized `ptext` uses the same `${money}` placeholder in its translation.

`set name value` keeps its previous meaning: store a literal string. `set name=expression` calculates a number. `cset name expression` and `cset name=expression` always calculate. Quote an expression containing spaces. Arithmetic can produce fractions; use `floor`, `ceil` or `round` before passing a result to an action that requires an integer count. Each line assigns one variable. Dialog variables belong to the current dialog, unlike persistent story flags.

## Expression language

Numbers use a decimal point regardless of the selected language. Use `${VariableName}` for a case-sensitive dialog variable. Reading a variable reads its numeric value; its text cannot inject additional syntax.

- Arithmetic: `+ - * / % ^`, parentheses, unary `+ -`.
- Comparison: `< <= > >= == !=`; booleans are `0` and `1`.
- Logic: `! && ||`, lazy `condition ? yes : no` and `if(condition, yes, no)`.
- Functions: `min`, `max`, `abs`, `floor`, `ceil`, `round`, `sqrt`, `clamp`, `rnd`, `rand`, `chance`.

Multiplication precedes addition. Powers associate to the right; `-2^2` is `-4`. `rnd` requires two ordered Int32 integer bounds. `rand` retains the existing stats behavior: reorder bounds, round inward and select an integer; `chance(p)` uses a percentage from 0 to 100.

Assignments reject undefined or nonnumeric variables, zero division, invalid arguments and non-finite results. The dialog stops with a source/line diagnostic before dependent actions run. The previous variable is not overwritten. Parsing, validation and the command palette inspect syntax without consuming randomness.

Stats formulas retain their previous permissive behavior (unknown variables and division by zero produce zero). Existing `Cthangover.Mods.Stats.Formulas` APIs remain available from this library for source compatibility.

## Mod-defined strategies

Depend on `expressions_base>=1`. A public parameterless strategy in a normal mod source file is discovered when its assembly loads and removed when it unloads:

```csharp
namespace ExampleMod
{
    using System.Collections.Generic;
    using Cthangover.Mods.Expressions;
    using Cthangover.Mods.Stats.Formulas;

    public class DoubleFunction : IExpressionFunctionStrategy
    {
        public string Name => "double_value";
        public int MinArguments => 1;
        public int MaxArguments => 1;
        public double Evaluate(ExpressionEvaluationContext context,
            IReadOnlyList<FormulaNode> arguments) => context.Value(arguments[0]) * 2;
    }
}
```

Now `cset score double_value(6)+1` stores `13`. Evaluate operands through `context.Value` to preserve strictness and lazy evaluation. Use `context.Random` for the current evaluation's random source. `MaxArguments = -1` means no upper bound.

`IExpressionBinaryStrategy` and `IExpressionUnaryStrategy` similarly provide a symbol, precedence and evaluation method. Binary strategies can declare right associativity. Symbols contain punctuation from `+-*/%^<>=!&|~`; precedence is 1–100, higher binds tighter. Built-in precedence: `||` 1, `&&` 2, comparisons 3, addition 4, multiplication 5, prefix 6, power 7. The longest registered operator token wins during parsing. First registration owns a name/symbol; duplicate registration cannot replace a built-in. Cached trees retain names rather than plugin instances, and registration changes invalidate parser caches.

DSL commands remain separate `IScenarioCommandStrategy` implementations. Their shared `IScenarioCommandSpec` contracts control argument validation, case preservation and canonical arguments for schema checking, without adding command-specific branches to the parser.

## Verification

`--test mod_units` exercises legacy formulas, arithmetic, inclusive random endpoints, invalid inputs, lazy branches, extension registration/unloading, parser caches and localized text/action reuse. `winter` and `winter_iso` run the reward in a real chest and discover an expression strategy from a separately compiled test mod. `game_novel_template` boots the same dependency chain without RPG.
