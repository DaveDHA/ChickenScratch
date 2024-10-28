# ChickenScratch

ChickenScratch is intended to be a set of tools that help produce reasonable-looking output quickly and easily from F# scripts and Polyglot Notebooks.  The main feature that it offers is a set of Html Expressions that allow easy creation of Html content.  Although it uses computation expressions rather than functions acting on lists, it is similar in nature to the [Giraffe View Engine](https://github.com/giraffe-fsharp/Giraffe.ViewEngine) or [Feliz View Engine](https://github.com/dbrattli/Feliz.ViewEngine), but with a few extra tricks that align with the goal of quick-and-dirty output from F# scripts and Notebooks, including:

- It allows any type that implements `IScratchNodeSource` to be dropped directly into Html Expressions.
- It allows content rendered with the "text/html" formatter registered with a Polyglot Notebook to be mixed in with Html Expressions.

ChickenScratch (and ChickenScratch.Interactive) also includes utilities for:

- Parsing values from a TabularDataResource in Polyglot Notebooks.
- Converting a TabularDataResource into a Microsoft DataFrame.
- Hiding cells from Html Exports of Polyglot Notebooks.

Please refer to [the documentation](https://github.com/DaveDHA/ChickenScratch/wiki) to get started.

This is very much a work in progress, and please remember that the focus is on quick and easy output from support scripts, not on being the most robust HTML library available.