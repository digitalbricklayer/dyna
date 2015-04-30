# dyna
A project to explore how to visualise a constraint satisfaction problem with an engine for presenting a solution. Uses the [Google or-tools](https://developers.google.com/optimization/) for resolving the problem.

The program idea is outlined in [The strange case of the missing application](http://techteapot.com/strange-case-of-the-missing-application/). dyna is an attempt to build the missing application, to see if such a thing exists, or has *any* utility. In the past whenever I've tried to come up with a suitable interface I've always eventually come up with something a lot like a spreadsheet. Just goes to show just how good a paradigm the spreadsheet is.

Plainly a big barrier to any kind of constraint satisfaction project is going to be the fact that they are [NP-hard](http://en.wikipedia.org/wiki/NP-hard) problems. In other words, they tend to be *very* hard to solve quickly. I'm kind of working on the assumption that Moore's Law is going to rescue me. Quantum computers would appear to be ideally placed to solve constraint satisfaction type problems. So, I am just going to assume that, at some point in the future, the hardware will be there to solve even very complex constraint satisfaction problems in a reasonable amount of time.

Design goals:
* **Technical level** - the user of the software should require no programming experience;
* **User Interface** - the problem should be described in a graphical way;
* **Build a playground** in which various types of constraint type problems can be built and solved in an interactive manner.

Please do not use this project for anything other than experimentation. I make no guarantees about backward compatibility or indeed anything else.
