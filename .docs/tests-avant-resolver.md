# Tests du parser à écrire avant le resolver

Le resolver part du principe que la forme de l'AST est correcte. Si le parser crée un nœud du mauvais type ou oublie un sous-arbre, le resolver ne résout pas la variable concernée et aucune erreur ne le signale.

Pour marquer un test comme terminé, coche la case `[x]`.

## Priorité 1 — à terminer avant le resolver

| Fait | # | Entrée | Attendu | Pourquoi c'est important pour le resolver |
|:---:|---|---|---|---|
| [ ] | 1 | `a = b = c` | `AssignExpression` dont `Value` est un `AssignExpression` (associativité à droite) | `a` et `b` sont deux affectations à résoudre, `c` est une lecture |
| [ ] | 2 | `a.b = 1` | `SetExpression` (`Object` = `a`, `Value` = `1`), et non `AssignExpression` | `b` est une propriété : il ne faut pas la chercher dans les portées |
| [ ] | 3 | `1 = 2` | erreur « Invalid assignment target » | aucun nœud d'affectation ne doit atteindre le resolver sans une cible valide |
| [ ] | 4a | `foo()()` | `CallExpression` dont `Callee` est un `CallExpression` | si le parser n'avale qu'un suffixe, les arguments suivants ne sont jamais résolus |
| [ ] | 4b | `a.b(1)(2)` | `Call(Call(Get(a, b), [1]), [2])` | idem |
| [ ] | 5 | `a and b or c` | `LogicalExpression` à la racine | c'est un autre type de nœud, que le resolver doit parcourir |
| [ ] | 6a | `a == b == c` | `BinaryExpression` dont `Left` est un `BinaryExpression` | un opérande perdu est une variable jamais résolue |
| [ ] | 6b | `10 - 3 - 2` | `(10 - 3) - 2` (associativité à gauche) | idem |
| [ ] | 6c | `a * b * c` | `(a * b) * c` | idem (`FactorExpressionParser` n'a pas encore été vérifié) |
| [ ] | 7a | `var a;` dans un bloc | `VarDeclarationStatement`, initialiseur `null` | cas typique déclarée mais non initialisée |
| [ ] | 7b | `var a;` au niveau du programme | idem | idem |
| [ ] | 8 | `foo();` | `ExpressionStatement` → `CallExpression`, un seul niveau d'enfants | c'est le point d'entrée du parcours des expressions |
| [ ] | 9 | `if (x) print 1;` | `IfStatement` avec `ElseBranch == null`, zéro erreur | vérifie qu'il n'y a pas de `null` silencieux à la place du nœud |

### Décision à prendre

| Fait | # | Entrée | Attendu | Remarque |
|:---:|---|---|---|---|
| [ ] | 10 | `(a) = 1;` | rejeté (« Invalid assignment target ») | nécessite le nœud Grouping, qui serait un type de nœud de plus à parcourir pour le resolver. Soit on l'implémente maintenant, soit on le note comme limite connue |

## Priorité 2 — peut attendre (uniquement des messages d'erreur du parser)

| Fait | # | Entrée | Attendu |
|:---:|---|---|---|
| [ ] | 11 | `a = ;` | exactement une erreur, pas de nœud |
| [ ] | 12 | `f(1,` | exactement une erreur |
| [ ] | 13 | `f(1 2)` | erreur « Expect ')' after arguments » |
| [ ] | 14 | `a.` suivi de `;` | erreur « Expect property name after '.' » |
| [ ] | 15 | `if (x) }` | exactement une erreur (émise par `StatementParser`) |
| [ ] | 16 | `a.b + 1` | `BinaryExpression` dont `Left` est un `GetExpression` |
| [ ] | 17 | `f(1, 2)` / `f()` | deux arguments / zéro argument |
| [ ] | 18 | `1 < 2` | un `BinaryExpression`, aucune erreur |
| [ ] | 19 | `foo(1 + 2)` | le 2ᵉ enfant est bien le nœud `+`. Ce test passait jusqu'ici sans rien vérifier réellement |

## Rangement

- [ ] Supprimer le doublon `InlineData` dans `CodeTest.When_Zero_Then_No_Error_Is_Returned` (avertissement xUnit1025).

## À écrire au début du resolver

| Fait | Portée | Code | Attendu |
|:---:|---|---|---|
| [ ] | globale | `var a = 1; var a = 2;` | accepté |
| [ ] | globale | `var a = a;` | accepté (lit l'ancienne globale) |
| [ ] | locale (bloc) | `{ var a = 1; var a = 2; }` | « Already a variable with this name in this scope » |
| [ ] | locale (bloc) | `{ var a = a; }` | « Can't read local variable in its own initializer » |
