An attempt to try the specification pattern described here

https://github.com/HuddleEng/Specification/blob/main/docs/All-about-security.md

in fsharp


When run this gives the following:

```
Answer is true from "(<fun:canDeleteDocument@60> OR <fun:canDeleteDocument@60-1>)"
Answer is false from "(<fun:canDeleteDocument@60> OR <fun:canDeleteDocument@60-1>)"
Answer is true from "(<fun:canReadDocument@57> OR <fun:canReadDocument@57-1>)"
Answer is true from "(<fun:canReadDocument@57> OR <fun:canReadDocument@57-1>)"
Answer is true from "((<fun:canReadDocument@57> OR <fun:canReadDocument@57-1>) AND (<fun:canDeleteDocument@60> OR <fun:canDeleteDocument@60-1>))"
Answer is false from "((<fun:canReadDocument@57> OR <fun:canReadDocument@57-1>) AND (<fun:canDeleteDocument@60> OR <fun:canDeleteDocument@60-1>))"
```

## Problems

1. How to compose the isSatisfiedBy function and pretty print together in a nice way
2. How to add meta data to the functions so that pretty print shows the name of the function.