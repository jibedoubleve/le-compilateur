window.registerLox = () => {
    monaco.languages.register({ id: 'lox' });
    monaco.languages.setMonarchTokensProvider('lox', {
        keywords: ['and', 'class', 'else', 'false', 'for', 'fun', 'if', 'nil', 'or',
            'print', 'return', 'super', 'this', 'true', 'var', 'while'],
        tokenizer: {
            root: [
                [/\/\/.*$/, 'comment'],
                [/\/\*/, 'comment', '@comment'],
                [/"/, 'string', '@string'],
                [/\d+(\.\d+)?/, 'number'],
                [/[a-zA-Z_]\w*/, { cases: { '@keywords': 'keyword', '@default': 'identifier' } }]
            ],
            comment: [[/[^*]+/, 'comment'], [/\*\//, 'comment', '@pop'], [/\*/, 'comment']],
            string: [[/[^"]+/, 'string'], [/"/, 'string', '@pop']]
        }
    });
    monaco.editor.getModels().forEach(model => monaco.editor.setModelLanguage(model, 'lox'));
};