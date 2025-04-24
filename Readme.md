# DiceRoller

A command-line wrapper over the `DiceRollerCore` library, designed not as a full-fledged application, but as a demonstration and utility layer for showcasing the library's capabilities. It allows for both quick usage and advanced integration of virtual dice roll mechanics.

---

## Implemented Features

- **Virtual Dice Rolls (`dice roll <formula>`)**: Simulate dice rolls, such as `2d20highest + d4 + 5`, including arithmetic operations and comparisons.  
- **Probability Analysis (`dice analyze <formula>`)**: Instead of rolling a single result, compute the probabilities of all possible outcomes.  
- **CLI Interface**: Easy-to-use commands and options accesible from any terminal.  

---

## Installation

### Option 1: Stable Windows Installer

Download the latest **stable release** from the [Releases](https://github.com/Qumaa/dice-parser/releases) tab. It includes a ready-to-use setup wizard and uninstaller.

### Option 2: Clone and Build Manually

```bash
git clone https://github.com/Qumaa/dice-parser.git
```

Then build the project:

```bash
cd DiceRoller  
dotnet build -c Release
```

To make it globally accessible on Windows, add the output directory to your system `PATH`:

- Open Environment Variables  
- Add the output folder (e.g., `DiceRoller\bin\Release\netX.X`) to your user `PATH`  
- Restart your terminal  

---

## Formula Syntax

### Operands

- **Number**  
  Any whole number within the 32-bit signed integer range (–2 147 483 648 through 2 147 483 647).

- **Dice**  
  Format: `[count]d[sides][composer]`  
  - `count`: Optional number of dice (default is 1)  
  - `d`: Dice notation delimiter  
  - `sides`: Number of sides per die (required)  
  - `composer`: Optional result modifier:  
    - `h`, `highest` – highest result  
    - `l`, `lowest` – lowest result  
    - `s`, `sum`, `summation` or not specified – sum of all results  
  - Examples:  
    - `d6` → roll one six-sided die  
    - `2d20h` → roll two d20s and take the highest  
    - `3d8l` → roll three d8s and take the lowest  
    - `4d6s`/`4d6` → roll four six-sided dice and sum them  

  **Analysis behavior:** Displays the full probability distribution of all possible roll results.  
  **Roll behavior:** Displays a single computed roll result.

### Arithmetic Operators

Perform standard numeric operations on dice and number operands.

- `+` – Addition  
- `-` – Subtraction  
- `*` – Multiplication  
- `/` – Division, always round **down**  
- `//` – Division, always round **up**  

**Analysis behavior:** Displays the full probability distribution of all possible roll results.  
**Roll behavior:** Displays a single computed roll result.

### Relational Operators

Compare dice and number operands and return boolean outcomes.

- `>` – Greater than  
- `<` – Less than  
- `>=` – Greater than or equal  
- `<=` – Less than or equal  
- `=`, `==` – Equal  
- `!=`, `=/=` – Not equal

**Analysis behavior:**  
- Filters out all failing roll results  
- Displays probability for each successful outcome  
- Accumulates and shows total success/failure probabilities  

**Roll behavior:**  
- Displays whether the condition passed or failed and shows a roll result or a failure message  

### Logical Operators

Operate on boolean expressions.

- `!`, `not` – Logical NOT  
- `&`, `&&`, `and` – Logical AND  
- `|`, `||`, `or` – Logical OR

**Analysis behavior:**  
- Cannot determine individual result values  
- Only shows the total probability of success or failure  

**Roll behavior:**  
- Displays only a success/failure message  
- Does not show numeric results

---

## Roadmap

- **[High Priority] Tree Visualization (`dice roll <expression> --tree`)**  
  Graph-style breakdown of expressions and operator hierarchy.  
  _The core library will be updated._

- **Ternary Operator Syntax**  
  Support for syntax like `condition ? true : false` that continues with either result or `condition ? true` that returns false to the outer scope of the formula. Nesting ternary operators is planned to be supported.   
  _The core library will be updated._

- **"Capturing" Syntax**  
  Mechanisms for temporarily storing roll results, reusing, and re-rolling them within the formula.  
  Think of attack rules of tabletop RPGs, like D&D 5e and Pathfinder 2e that have several checks for a singular roll to determine the output.

- **Variables Syntax (optional)**   
  An expansion on the "capturing" syntax to store arbitrary values during the evaluation.    
  Think of critical success/failure levels in Pathfinder.

- **`.dice` File Support**  
  Allow loading complex formulas with positional arguments from files instead of inline CLI input. The positional arguments will be implemented with the variables syntax, hence their implementation is not conclusive.

- **Extended Expression Composition Operators**  
  Current support is limited to dice expressions in form of \<dice>d\<faces>\<composition>, e.g. 2d20highest. Will be expanded to support any subexpression like `2 2d20highest lowest`.    
  Think of variable amount of dice to roll, like critical hits damage.  

  This change will also make the currently internal API for implementing operators beyond regular binary and unary variants public. As of now, it needs testing and polishing.

- **NuGet Publication**  
  The core library (`DiceRollerCore`) will be published as a NuGet package after all significant updates mentioned above.

- **Alias System**  
  A toolset for managing named expressions or macros that can be reused inside formulas. For example, defining `2d6 + 4` as `melee` and typing `dice roll melee` would pre-process the input to be equal to `dice roll 2d6 + 4`.

- **Extensions Support**  
  Creating a framework to easily extend the application functionality in C# and markup languages - defining tokens/syntax table of operators and operands, overriding operator handlers operand parsers to implement exotic scenarios beyond built-in options, composition handlers, etc..

- **Profiles**  
  A system for exporting, importing, and managing tokens/syntax tables, extensions, and aliases as a "profile".  
  For example, a TTRPG game-specific profile may add an `atk` operator with custom C#-written logic that resolves to full attack mechanics and damage resolution of the game.

- **Localization (Optional)**  
  Will be considered after all major features are in place.

---

## License

MIT License. Feel free to fork, build, and contribute.

---

## Feedback

Found a bug or have a feature request? Open an [issue](https://github.com/Qumaa/dice-parser/issues) or start a discussion!
