# Parsing (analyse syntaxique)

## Rôle dans le pipeline

Le parser prend le flux de tokens produit par le lexer et construit un
arbre (AST — *Abstract Syntax Tree*) qui représente la structure
grammaticale du programme. Le lexer répond à "quels sont les mots ?",
le parser répond à "comment ces mots s'assemblent-ils en phrases ?".

Une différence importante avec le lexing : le lexer travaille sur un
alphabet fini et des règles régulières (automates finis). Le parser
travaille sur des **grammaires hors-contexte** (context-free grammars,
CFG), un modèle strictement plus puissant — c'est ce qui permet de
représenter la récursion et l'imbrication arbitraire (parenthèses
imbriquées, expressions dans expressions), ce qu'un automate fini ne
peut pas faire.

## Grammaires hors-contexte (CFG)

Une grammaire est un ensemble de règles de production, écrites en
notation BNF ou EBNF :

- **Terminal** : un token concret (`"+"`, `NUMBER`, `IDENTIFIER`).
- **Non-terminal** : un symbole abstrait qui se développe via une
  règle (`expression`, `term`).
- **Production** : une règle de la forme `nonterminal → séquence`.
- **Dérivation** : l'application successive de règles pour passer du
  symbole de départ à une chaîne de terminaux.

Notation EBNF utilisée par le livre (et par ce projet) :

| Symbole | Signification |
|---|---|
| `\|` | alternative (ou) |
| `*` | zéro ou plusieurs répétitions |
| `+` | une ou plusieurs répétitions |
| `?` | optionnel (zéro ou une fois) |
| `( … )` | groupement |

Exemple, la grammaire des expressions Lox (ordre = précédence, du plus
faible au plus fort) :

```
expression → assignment ;
assignment → IDENTIFIER "=" assignment | logic_or ;
logic_or   → logic_and ( "or" logic_and )* ;
logic_and  → equality ( "and" equality )* ;
equality   → comparison ( ( "!=" | "==" ) comparison )* ;
comparison → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
term       → factor ( ( "-" | "+" ) factor )* ;
factor     → unary ( ( "/" | "*" ) unary )* ;
unary      → ( "!" | "-" ) unary | call ;
call       → primary ( "(" arguments? ")" | "." IDENTIFIER )* ;
primary    → NUMBER | STRING | "true" | "false" | "nil"
           | "(" expression ")" | IDENTIFIER | "super" "." IDENTIFIER ;
```

## Deux grandes familles de parseurs

| | Approche | Idée | Exemples |
|---|---|---|---|
| **Descendant (top-down)** | Part du symbole racine, essaie de le dériver jusqu'aux tokens observés | Récursive descente, LL(k), PEG | GCC (C++), Clang, Roslyn (C#), CPython (PEG) |
| **Ascendant (bottom-up)** | Part des tokens, les réduit progressivement vers la racine | LR, LALR (yacc/bison) | Parsers générés (grammaires complexes, langages avec ambiguïtés lourdes) |

Les compilateurs de production écrits à la main (donc pas générés par
un outil comme ANTLR/yacc) utilisent presque toujours la **récursive
descente** — c'est l'approche du livre, et celle de ce projet.

## Récursive descente

Principe : **une fonction (ou classe) par règle de grammaire**. Chaque
non-terminal de la grammaire devient une unité de code qui sait
reconnaître ("match") et consommer les tokens correspondant à sa
règle, en délégant aux non-terminaux qu'elle référence.

Correspondance directe grammaire → code dans ce projet :

| Règle de grammaire | Classe |
|---|---|
| `assignment` | `AssignmentExpressionParser` |
| `logic_or` | `OrExpressionParser` |
| `logic_and` | `AndExpressionParser` |
| `equality` | `EqualityExpressionParser` |
| `comparison` | `ComparisonExpressionParser` |
| `term` | `TermExpressionParser` |
| `factor` | `FactorExpressionParser` |
| `unary` | `UnaryExpressionParser` |
| `call` | `CallExpressionParser` |
| `primary` | `PrimaryExpressionParser` |

C'est une **prédiction sur un seul token de lookahead** (LL(1)) : à
chaque étape, on regarde le token courant (`Peek`) pour décider quelle
règle appliquer, sans backtracking.

## Précédence et associativité — le point le plus important

La grammaire ci-dessus encode la précédence **par la profondeur
d'imbrication des règles** : `equality` appelle `comparison`, qui
appelle `term`, etc. Plus une règle est "loin" de `expression`, plus
son opérateur est prioritaire (`*` se lie plus fort que `+`, qui se
lie plus fort que `==`).

Ce projet implémente ça via une chaîne de responsabilité générique :
chaque parseur de niveau de précédence hérite de
`PrecedenceParser<TChildParser>`, où `TChildParser` est le niveau de
précédence immédiatement supérieur. `Matches` délègue automatiquement
vers l'enfant si l'opérateur du niveau courant n'est pas présent.
C'est une variante orientée-objet du patron classique "precedence
climbing" — alternative connue : le **parsing Pratt** (table de
"binding powers" associée à chaque opérateur, un seul point d'entrée
récursif plutôt qu'une classe par niveau). Les deux techniques
résolvent le même problème ; Pratt est plus compact mais moins
explicite sur la grammaire sous-jacente.

### Associativité : boucle vs récursion — piège classique

Une règle `X → Y ( op Y )*` (opérateur `*`) est **associative à
gauche** et doit être traduite par une **boucle** qui accumule un
nœud AST à chaque itération (voir `EqualityExpressionParser.Parse`,
implémenté par récursion terminale avec accumulateur — équivalent
sémantique d'une boucle). Une règle `X → op X` (le `X` de droite se
référence lui-même directement, sans `*`) est **associative à
droite** et se traduit naturellement par un **appel récursif direct**
(voir `UnaryExpressionParser`, pour `--a` = `-(-a)`).

Confondre les deux — traduire une règle `*` par un simple `if` au
lieu d'une boucle/récursion accumulatrice — produit un parser qui
n'accepte qu'une seule occurrence de l'opérateur au lieu d'une chaîne
(`a == b == c` casse silencieusement). C'est un bug qui ne casse pas à
la compilation : il faut un test avec au moins deux occurrences de
l'opérateur pour le détecter.

## Récursion à gauche : le danger à connaître

Une règle comme `expr → expr "+" term` (le non-terminal s'appelle
*lui-même en première position*) boucle infiniment en récursive
descente naïve (la fonction s'appelle elle-même avant de consommer un
seul token). C'est pour ça que la grammaire EBNF ci-dessus réécrit
systématiquement les règles binaires en `Y ( op Y )*` — cette forme
est équivalente mais élimine la récursion à gauche au profit d'une
boucle. À retenir : **toute règle grammaticale récursive à gauche doit
être réécrite avant d'être traduite en récursive descente.**

## Postfixes et chaînage (`call`)

La règle `call → primary ( "(" arguments? ")" | "." IDENTIFIER )*`
gère les appels et accès de champs **chaînés** (`foo()()`,
`a.b.c()`) via une boucle sur les opérateurs postfixes après avoir
parsé le `primary`. C'est structurellement le même patron que
`equality`/`term` (boucle après avoir parsé l'opérande de plus haute
précédence) — la seule différence est que l'opérateur est un token
implicite (`(` ou `.`) plutôt qu'un opérateur binaire explicite.

## Gestion des erreurs

Deux stratégies classiques :

- **Mode panique (panic mode) + synchronisation** : à la première
  erreur, on abandonne la règle courante et on avance le curseur
  jusqu'à un point "sûr" pour reprendre (typiquement le prochain
  `;` ou mot-clé de déclaration). Permet de rapporter plusieurs
  erreurs en une seule passe plutôt que de s'arrêter à la première.
- **Arrêt immédiat** : plus simple, mais UX de compilateur médiocre
  (un seul message d'erreur par compilation).

Le livre (et la structure de ce projet, avec `ParsingContext.AddError`)
suit la première approche, sans forcément implémenter la
synchronisation complète — à vérifier/décider selon l'avancement.

## Glossaire rapide

| Terme | Définition |
|---|---|
| Terminal | Token concret, feuille de l'arbre |
| Non-terminal | Symbole abstrait qui se développe via une règle |
| Production | Règle `non-terminal → séquence` |
| Dérivation | Application successive de règles |
| Ambiguïté | Une même chaîne de tokens a plusieurs arbres de dérivation valides |
| Lookahead | Nombre de tokens regardés en avance pour décider quelle règle appliquer |
| Backtracking | Revenir en arrière après un essai raté (récursive descente classique n'en fait pas — LL(1) prédictif) |
| Precedence climbing | Technique encodant la précédence via l'imbrication de règles/fonctions |
| Parsing Pratt | Variante utilisant une table de "binding power" par opérateur plutôt qu'une fonction par niveau |

## Repères pro

- **Où ça apparaît en production** : le parser C# de Roslyn, le
  frontend C++ de Clang, le parser JavaScript de V8, le nouveau
  parser PEG de CPython (3.9+) sont tous des récursive-descente
  écrits à la main — pas des parsers générés. C'est l'approche
  dominante dès qu'on veut de bons messages d'erreur et un contrôle
  fin de la récupération après erreur.
- **Entretiens techniques** : "écris un parser d'expressions
  arithmétiques avec précédence" est un exercice classique de
  round système/bas niveau. Les questions pièges tournent autour de
  la récursion à gauche et de l'associativité gauche/droite.
- **Usages quotidiens hors compilateurs** : parseurs de requêtes
  (SQL, GraphQL, OData), moteurs de templates, parseurs de fichiers
  de config (avec expressions, pas juste clé/valeur), linters et
  analyseurs statiques, moteurs de règles métier — partout où un
  texte doit être transformé en structure avant d'être interprété.
