# Resolver (résolution de portée et de liaison)

## Rôle dans le pipeline

Le resolver se place **entre le parsing et l'exécution** : il fait une
passe complète sur l'AST *avant* l'interprétation, pour déterminer, pour
chaque référence à une variable, **à quelle déclaration exacte elle se
lie**. Contrairement au lexer et au parser (analyse purement
syntaxique), le resolver fait de l'**analyse sémantique** : il donne un
sens de "liaison" (*binding*) aux identifiants, indépendamment de leur
exécution.

Ce n'est pas une étape optionnelle de confort : sans elle, un
interpréteur naïf qui se contente de chercher un nom dans la chaîne
d'environnements au moment de l'exécution produit des résultats
**incorrects** en présence de closures. C'est le problème central que
le resolver résout.

## L'intuition qui casse : portée lexicale vs résolution dynamique

Réflexe venant du génie logiciel général : "chercher le nom dans la
portée courante, remonter la chaîne de scopes si pas trouvé" — exactement
comme une chaîne de prototypes ou un `Dictionary` imbriqué. Ce réflexe
est correct... au moment où le code s'exécute. Le problème, c'est que
Lox promet une **portée lexicale** : une closure doit capturer les
variables telles qu'elles existaient *à l'endroit où elle a été
déclarée dans le texte source*, pas telles qu'elles se trouvent être
au moment de l'appel.

Exemple canonique (celui du livre) :

```
var a = "global";
{
  fun showA() { print a; }
  showA();          // doit afficher "global"
  var a = "block";
  showA();           // doit AUSSI afficher "global"
}
```

Si on résout `a` dynamiquement à chaque appel de `showA` (en remontant
la chaîne d'environnements *courante*), le second appel voit le `a`
local nouvellement déclaré et affiche "block" — violation de la portée
lexicale. Le nombre de scopes entre `showA` et une déclaration de `a`
a changé *après* que la closure a été créée, alors que le texte source,
lui, n'a pas changé. Le resolver calcule cette liaison **une seule
fois, statiquement**, à partir de la structure du code, avant toute
exécution — donc le résultat est fixe et indépendant du "quand" du
runtime.

## Modèle : pile de scopes + declare/define en deux temps

Le resolver marche l'AST avec une pile de dictionnaires
`nom → bool` (un dictionnaire par bloc/scope actif) :

- **`declare`** : le nom entre dans le scope courant, marqué comme "pas
  encore prêt".
- **`define`** : le nom est marqué "prêt à être utilisé".

Séparer ces deux étapes permet de détecter statiquement l'auto-référence
dans un initialiseur (`var a = a;` — le `a` à droite ne doit pas
pouvoir résoudre vers le `a` qu'on est en train de déclarer) : si on
rencontre une référence à un nom "declared but not defined" dans le
scope courant, c'est une erreur statique.

**Le scope global n'est pas suivi dans cette pile** — il est résolu
dynamiquement par nom à l'exécution, comme avant. La raison : Lox
autorise la redéfinition libre au niveau global (utile pour le REPL,
où chaque ligne est évaluée indépendamment). Seuls les scopes
imbriqués (blocs, fonctions, méthodes) ont besoin d'une résolution
statique par distance.

## Ce que le resolver vérifie en plus (erreurs statiques)

En profitant du fait qu'il fait déjà une passe complète sur l'AST,
le resolver est l'endroit naturel pour détecter, *avant l'exécution*,
des erreurs qu'un interpréteur naïf ne verrait qu'au runtime (ou pas
du tout) :

- `return` utilisé en dehors d'une fonction.
- `this` utilisé en dehors d'une classe.
- `super` utilisé en dehors d'une sous-classe, ou dans une classe sans
  superclasse.
- Une classe qui hérite d'elle-même.
- Variable locale déclarée mais jamais utilisée (optionnel selon le
  niveau de rigueur voulu — le livre le propose en exercice).

C'est la même logique que les erreurs de compilation en C#/Java :
détectées avant l'exécution parce que l'information nécessaire est
disponible statiquement.

## Comment l'information résolue est utilisée à l'exécution

Le resolver produit, pour chaque nœud "référence de variable", une
**distance** : le nombre de scopes à remonter depuis le point d'usage
pour atteindre le scope où la variable a été déclarée. À l'exécution,
l'interpréteur n'a plus besoin de chercher le nom par tâtonnement dans
la chaîne d'environnements : il saute directement à l'environnement
ancêtre à la bonne distance (dans le livre : `environment.getAt(distance,
name)`), et ne retombe sur une recherche par nom que pour le cas
global (distance absente = variable globale).

## Correspondance attendue avec les rôles de nœuds AST existants

Aucun code de resolver n'existe encore dans ce projet, mais la
structure de `SyntaxNodeRole` donne déjà la forme du travail à faire —
chaque rôle qui introduit ou référence un scope a besoin d'un
traitement dédié dans le resolver, sur le même principe "une unité de
traitement par catégorie de nœud" que le parser :

| `SyntaxNodeRole` | Ce que le resolver doit faire |
|---|---|
| `Var` | `declare` puis résoudre l'initialiseur puis `define` |
| `Function` | `declare` + `define` le nom de la fonction, puis nouveau scope pour les paramètres et le corps |
| `Class` | nouveau scope pour `this` (et `super` si héritage) ; résoudre chaque méthode comme une fonction |
| `Body` / blocs | pousser un nouveau scope, résoudre les déclarations enfants, dépiler |
| `Call` | résoudre le callee et chaque argument (pas de nouveau scope) |
| `Get` | résoudre l'objet ; le nom de propriété n'est *jamais* résolu comme une variable (résolution dynamique par nom sur l'instance, jamais statique) |
| `Condition` / `Then` / `Else` | résoudre les **deux branches**, inconditionnellement |

## Pièges classiques

- **Parcours inconditionnel, contrairement à l'interprétation.** Le
  resolver n'a pas de notion de "vrai/faux à l'exécution" : il visite
  *toujours* les deux branches d'un `if`, le corps d'une boucle une
  seule fois (pas en boucle), etc. — c'est une analyse statique du
  texte, pas une simulation d'exécution. Modéliser le resolver comme
  "un mini-interpréteur" est le contresens le plus courant.
- **Une propriété (`Get`, après un `.`) ne se résout jamais comme une
  variable.** `a.b` : `a` est résolu (recherche de portée), mais `b`
  est une propriété résolue dynamiquement sur l'objet à l'exécution,
  jamais via la pile de scopes lexicaux. Mélanger les deux casse le
  duck-typing des instances Lox.
- **Scope de classe pour `this`** : il faut un scope intermédiaire
  entre la classe et ses méthodes uniquement pour y injecter `this`
  (et un autre encore pour `super` si héritage) — sinon `this` à
  l'intérieur d'une méthode ne résout nulle part.

## Piège spécifique à ce projet : `SyntaxNode` est un `record`

Le livre implémente la table de résolution comme
`Map<Expr, Integer>` **indexée par identité d'objet** (deux nœuds
distincts en mémoire sont toujours des clés distinctes, même s'ils
sont syntaxiquement identiques). Dans ce projet, `SyntaxNode` est un
`record` C# — qui a une **égalité structurelle** par défaut (deux
instances avec les mêmes valeurs de propriétés sont égales, et ont le
même hash code). Un port direct de l'approche du livre vers un
`Dictionary<SyntaxNode, int>` classique va donc **collisionner
silencieusement** : deux références distinctes à la même variable dans
deux endroits différents du code (`print a; print a;`) produisent des
`SyntaxNode` structurellement identiques (même `Token`, mêmes
`Children`), qui se retrouveraient à tort mappés à la même entrée.

Deux solutions typiques à ce problème, à trancher au moment de
l'implémentation : forcer une égalité par référence pour cette table
précise (ex. `ReferenceEqualityComparer` avec un `Dictionary`), ou
donner à chaque nœud un identifiant stable attribué à la création
(plutôt que de dépendre de l'identité de l'objet ou de sa structure).

## Glossaire rapide

| Terme | Définition |
|---|---|
| Portée lexicale (*lexical/static scoping*) | La liaison d'un identifiant dépend de sa position dans le texte source, fixée avant l'exécution |
| Portée dynamique | La liaison dépend de l'état d'exécution au moment de l'accès (ce que Lox *ne* veut *pas*) |
| Binding | L'association entre une référence à un identifiant et sa déclaration |
| Distance / depth | Nombre de scopes à remonter pour atteindre la déclaration d'une variable |
| Shadowing | Une déclaration locale masque une déclaration de même nom dans une portée englobante |
| Analyse statique | Analyse effectuée sans exécuter le programme |

## Repères pro

- **Où ça apparaît en production** : tout langage avec closures et
  portée lexicale (JavaScript, Python, C#) a un mécanisme équivalent
  dans son compilateur/runtime — le *binder* de Roslyn fait
  exactement ce travail pour C# (résolution des symboles avant la
  génération d'IL), V8 fait de l'analyse de scope à la compilation du
  bytecode pour décider si une variable doit vivre sur la pile ou être
  "boxée" dans un objet de contexte de closure (l'optimisation qui
  distingue les variables capturées des variables locales pures).
- **Bugs réels que ça évite** : les bugs classiques de closures dans
  des boucles (`for (var i ...) { setTimeout(() => print(i)) }` en
  JS avant `let`) viennent précisément d'une confusion entre portée
  de bloc et portée de fonction au moment de la résolution — même
  classe de problème que ce que le resolver formalise ici.
- **Entretiens techniques** : "pourquoi ce closure affiche la mauvaise
  valeur" est une question classique de round JS/langages
  dynamiques ; savoir expliquer la différence liaison statique vs
  dynamique en termes de mécanisme (pas juste "les closures sont
  bizarres") est ce qui distingue une bonne réponse.
