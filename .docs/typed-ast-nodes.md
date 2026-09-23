# AST typé : quel nœud produit chaque parser

## 1. AST homogène vs AST typé

| | AST homogène | AST typé |
|---|---|---|
| Forme | une seule classe `SyntaxNode` + un champ qui dit ce qu'elle est (`Role`, `Kind`) | une classe par forme syntaxique (`IfStatement`, `BinaryExpression`…) |
| Accès aux enfants | `Children[i]`, indexé par convention | propriétés nommées et typées (`Condition`, `ThenBranch`…) |
| Erreur de structure | détectée à l'exécution (mauvais index, enfant absent) | détectée à la compilation |
| Exhaustivité | un `switch` sur un enum oublie un cas en silence | un Visitor force chaque passe à traiter chaque type |
| Exemples | compilateur TypeScript (`node.kind`) | Roslyn, jlox (Nystrom) |

L'AST homogène est plus simple et plus uniforme (sérialisation, dump générique).
L'AST typé offre de la sécurité statique : c'est lui qui paie dès qu'il y a
plusieurs passes (resolver, interpréteur, type checker…).

## 2. Deux informations que `Role` mélangeait

Un ancien `Role` portait deux informations de nature différente :

| Information | Exemples | Où elle vit dans un AST typé |
|---|---|---|
| **Ce qu'est le nœud** | `Class`, `Function`, `Var`, `Call`, `Get` | le **type C#** du nœud |
| **La place qu'il occupe chez son parent** (*slot*) | `Condition`, `Then`, `Else`, `Init`, `Increment`, `Body` | le **nom de la propriété du parent** |

Conséquence : `Role` disparaît. Un nom de *slot* ne doit jamais devenir un
type de nœud. Il n'existe pas de `BodyNode` ni d'`InitializerNode` : un body est
un **block**, un initializer est une **expression**.

## 3. Règles de conception

1. **Une source de vérité.** Les propriétés typées font foi. La vue générique
   `Children` est *calculée* à partir d'elles (jamais stockée en double), et
   renvoie une collection vide, jamais `null`, quand il n'y a pas d'enfant.
2. **Un nœud par forme syntaxique, pas par opérateur.** `+`, `<=` et `==` partagent
   `BinaryExpression` : l'opérateur est porté par son token (`Token.Kind`).
3. **Un nœud par forme, pas par règle de grammaire.** Les règles `equality`,
   `comparison`, `term` et `factor` n'existent que pour encoder la précédence.
   Après le parsing, la précédence est dans la **forme de l'arbre**. C'est la
   différence entre *parse tree* (syntaxe concrète : un nœud par règle) et
   *AST* (syntaxe abstraite).
4. **Les catégories deviennent des bases abstraites.** Les non-terminaux qui ne
   sont qu'une alternative entre d'autres règles (`statement`, `declaration`,
   `expression`) n'ont pas de forme propre : ils deviennent `StatementNode` et
   `ExpressionNode`, jamais instanciés.
5. **Un nom qui déclare est un token, un nom qui lit est une expression.**
   - déclare : nom de variable, de fonction, de classe, de paramètre → `Token`
   - lit : `a` dans `print a;`, la superclass dans `class B < A` → `VariableExpression`
6. **Pas d'enveloppe vide.** Un nœud qui n'apporte aucune information de plus que
   son unique enfant (ex. un « argument » qui ne contient qu'une expression) n'a
   pas lieu d'être : l'enfant va directement dans la propriété du parent.

## 4. Type de retour d'un parser vs nœud produit

Deux niveaux distincts :

- le **type statique de retour** est la *catégorie* : `StatementNode?` ou `ExpressionNode?` ;
- l'**objet réellement renvoyé** est un nœud *concret* de cette catégorie.

Un niveau de précédence ne produit pas toujours son propre nœud : `term` face à
`1` seul renvoie directement le `LiteralExpression` remonté des niveaux
inférieurs. Chaque parser d'expression a donc pour type de retour
`ExpressionNode?`, jamais un type plus précis comme `BinaryExpression`.

Pour que le type suive la catégorie à travers une interface, le contrat du parser
doit être paramétré par la catégorie (`IParser<ExpressionNode>`,
`IParser<StatementNode>`). En C#, les types de retour covariants (C# 9) ne
s'appliquent qu'aux `override` de classe, **pas** aux implémentations d'interface.

## 5. Table de correspondance

### Déclarations et statements (type de retour : `StatementNode?`)

| Parser | Règle | Nœud(s) produit(s) | Données / enfants |
|---|---|---|---|
| `ProgramParser` | `program → declaration* EOF` | liste de statements | |
| `DeclarationParser` | `declaration → classDecl \| funDecl \| varDecl \| statement` | délègue (aucun nœud propre) | |
| `ClassDeclarationParser` | `"class" IDENTIFIER ( "<" IDENTIFIER )? "{" function* "}"` | `ClassDeclaration` | `Name` (token), `Superclass` (`VariableExpression?`), `Methods` (liste de `FunctionDeclaration`) |
| `FuncDeclarationParser` / `FunctionParser` | `"fun" IDENTIFIER "(" parameters? ")" block` | `FunctionDeclaration` | `Name` (token), `Parameters` (tokens), `Body` (liste de statements) |
| `VarDeclarationParser` | `"var" IDENTIFIER ( "=" expression )? ";"` | `VarDeclaration` | `Name` (token), `Initializer` (`ExpressionNode?`) |
| `StatementParser` | `statement → exprStmt \| forStmt \| …` | délègue (aucun nœud propre) | |
| `ExpressionStatementParser` | `expression ";"` | `ExpressionStatement` | `Expression` |
| `PrintStatementParser` | `"print" expression ";"` | `PrintStatement` | `Expression` |
| `ReturnStatementParser` | `"return" expression? ";"` | `ReturnStatement` | `Keyword` (token), `Value` (`ExpressionNode?`) |
| `IfStatementParser` | `"if" "(" expression ")" statement ( "else" statement )?` | `IfStatement` | `Condition`, `ThenBranch`, `ElseBranch?` |
| `WhileStatementParser` | `"while" "(" expression ")" statement` | `WhileStatement` | `Condition`, `Body` |
| `ForStatementParser` | `"for" "(" … ")" statement` | `ForStatement` **ou** désucrage | voir note |
| `BlockStatementParser` / `BlockParser` | `"{" declaration* "}"` | `BlockStatement` | `Statements` (liste de statements) |

**Note sur `for`.** Nystrom ne crée pas de nœud `for` : il le *désucre* en
`BlockStatement { initializer ; WhileStatement(condition, Block { body ; increment }) }`.
Garder un `ForStatement` dédié (`Initializer`, `Condition`, `Increment`, `Body`)
est aussi légitime : le désucrage simplifie les passes suivantes, le nœud dédié
préserve la forme source (utile pour les messages d'erreur et le formatage).

### Expressions (type de retour : `ExpressionNode?`)

| Parser | Règle | Nœud(s) produit(s) | Données / enfants |
|---|---|---|---|
| `ExpressionParser` | `expression → assignment` | délègue (aucun nœud propre) | |
| `AssignmentExpressionParser` | `( call "." )? IDENTIFIER "=" assignment \| logic_or` | `AssignExpression`, `SetExpression`, ou délègue | Assign : `Name` (token), `Value`. Set : `Object`, `Name` (token), `Value` |
| `OrExpressionParser` | `logic_and ( "or" logic_and )*` | `LogicalExpression` ou délègue | `Left`, `Operator` (token), `Right` |
| `AndExpressionParser` | `equality ( "and" equality )*` | `LogicalExpression` ou délègue | idem |
| `EqualityExpressionParser` | `comparison ( ( "!=" \| "==" ) comparison )*` | `BinaryExpression` ou délègue | `Left`, `Operator` (token), `Right` |
| `ComparisonExpressionParser` | `term ( ( ">" \| ">=" \| "<" \| "<=" ) term )*` | `BinaryExpression` ou délègue | idem |
| `TermExpressionParser` | `factor ( ( "-" \| "+" ) factor )*` | `BinaryExpression` ou délègue | idem |
| `FactorExpressionParser` | `unary ( ( "/" \| "*" ) unary )*` | `BinaryExpression` ou délègue | idem |
| `UnaryExpressionParser` | `( "!" \| "-" ) unary \| call` | `UnaryExpression` ou délègue | `Operator` (token), `Operand` |
| `CallExpressionParser` | `primary ( "(" arguments? ")" \| "." IDENTIFIER )*` | `CallExpression`, `GetExpression`, ou délègue | Call : `Callee`, `Paren` (token), `Arguments` (liste d'expressions). Get : `Object`, `Name` (token) |
| `PrimaryExpressionParser` | voir ci-dessous | un nœud feuille ou `GroupingExpression` | |

`primary → "true" | "false" | "nil" | NUMBER | STRING | IDENTIFIER | "this" | "(" expression ")" | "super" "." IDENTIFIER`

| Token rencontré | Nœud | Enfants |
|---|---|---|
| `true`, `false`, `nil`, nombre, chaîne | `LiteralExpression` | aucun (feuille) |
| identifiant | `VariableExpression` | aucun (feuille) |
| `this` | `ThisExpression` | aucun (feuille) |
| `super` `.` IDENTIFIER | `SuperExpression` | aucun : `Keyword` et `Method` (tokens) |
| `(` | `GroupingExpression` | `Expression` : le seul cas non-feuille |

**Pourquoi `and`/`or` ont leur propre nœud alors qu'ils ont la forme d'un
binaire.** Leur *sémantique d'évaluation* diffère : ils court-circuitent (l'opérande
droit n'est pas toujours évalué). Un nœud distinct permet à l'interpréteur de les
traiter sans cas particulier caché dans `BinaryExpression`.

**Pourquoi garder `GroupingExpression`.** Supprimer les parenthèses de l'AST est
légitime (la précédence est déjà dans la forme de l'arbre), mais en Lox
`(a) = 1;` doit être rejeté (« Invalid assignment target ») alors que `a = 1;`
est valide. Sans nœud de groupement, les deux cibles ont le même arbre.

## 6. Paramètre vs argument

| | Paramètre (*formal parameter*) | Argument (*actual argument*) |
|---|---|---|
| Où | dans la déclaration | au site d'appel |
| Nature | un nom (identifiant) | une valeur (expression) |
| Exemple | `a`, `b` dans `fun add(a, b)` | `1`, `x * 2` dans `add(1, x * 2)` |
| Représentation | `Token` | `ExpressionNode` |

## 7. Conventions de nommage

- Suffixe par catégorie : `…Expression`, `…Statement` (ou `…Declaration`). Le mot
  `Node` devient superflu sur les classes concrètes.
- `Kind` plutôt que `Type` pour la catégorie d'un token (`TokenKind`) : « type »
  sera réservé aux types du langage lors de l'analyse sémantique. Convention
  partagée par Roslyn, TypeScript et rust-analyzer (`SyntaxKind`).
- Orthographe américaine en .NET : `Initializer`, pas `Initialiser`.
- `VariableExpression` (ce que fait le nœud) plutôt qu'`IdentifierExpression`
  (sa forme lexicale) : les identifiants de déclaration restent des tokens.

## 8. Références

- Nystrom, *Crafting Interpreters*, ch. 5 (Representing Code) et annexe II (Generated Syntax Tree Classes).
- Roslyn : `SyntaxNode`, `StatementSyntax`, `ExpressionSyntax`, `SyntaxKind`.
- TypeScript : `Node` + `SyntaxKind`, AST homogène avec dispatch par `switch (node.kind)`.
