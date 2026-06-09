# le-compilateur

An educational compiler project focused on learning language design and compiler architecture.

# Lox Grammar

## Rules for the parser

| Family       | Examples                                |
| ------------ | --------------------------------------- |
| Expressions  | 2 + 3, x = 5, foo(), true               |
| Statements   | print, if, while, for, return, blocs {} |
| Declarations | var, fun, class                         |

### Expressions

| Level | Rule                 | Operator                      |
| ----- | -------------------- | ----------------------------- |
| 1     | Expression           | → assignment                  |
| 2     | AssignmentExpression | =                             |
| 3     | OrExpression         | or                            |
| 4     | AndExpression        | and                           |
| 5     | EqualityExpression   | == !=                         |
| 6     | ComparisonExpression | < > <= >=                     |
| 7     | TermExpression       | + -                           |
| 8     | FactorExpression     | \* /                          |
| 9     | UnaryExpression      | ! -                           |
| 10    | CallExpression       | () .                          |
| 11    | PrimaryExpression    | litteral, identifiers, (expr) |

### Statement

| Statement           | Syntax                                          |
| ------------------- | ----------------------------------------------- |
| ExpressionStatement | expression ;                                    |
| PrintStatement      | print expression ;                              |
| IfStatement         | if ( expression ) statement ( else statement )? |
| WhileStatement      | while ( expression ) statement                  |
| ForStatement        | for ( init ; condition ; increment ) statement  |
| ReturnStatement     | return expression? ;                            |
| BlockStatement      | { declaration\* }                               |

### Declaration

| Declaration      | Syntax                                            |
| ---------------- | ------------------------------------------------- |
| VarDeclaration   | var identifier ( = expression )? ;                |
| FunDeclaration   | fun identifier ( params? ) block                  |
| ClassDeclaration | class identifier ( < identifier )? { function\* } |

