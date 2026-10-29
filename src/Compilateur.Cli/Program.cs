/*
 * =============================
 * ==== How to use this cli ====
 * =============================
 * lox exec <path.lox>
 * lox repl
 */


using Compilateur.Cli.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(cfg => {
    cfg.AddCommand<ExecCommand>("exec");
    cfg.AddCommand<ReplCommand>("repl");
});

return await app.RunAsync(args);