## Acknowledgements

- [How to: Define a generic method with reflection emit](https://learn.microsoft.com/en-us/dotnet/fundamentals/reflection/how-to-define-a-generic-method-with-reflection-emit)
- [Dynamically invoking a generic method with Reflection in .NET C#](https://brianlagunas.com/dynamically-invoking-a-generic-method-with-reflection-in-net-c/)
- [Simplifying Dynamic and Consistent Service Registration in .NET Core](https://medium.com/@asad99/simplifying-dynamic-and-consistent-service-registration-in-net-core-fd423c3ca4fe)
- [Replace Nested Conditional with Guard Clauses](https://refactoring.guru/replace-nested-conditional-with-guard-clauses)

### 🔥 **NESTING HELL**

```javascript
function foo() {
    if (dbConnected) {
        if (userLogged) {
            if (isAdmin) {
                console.log('Success!')
            } else {
                console.error('Only for admins!')
            }
        } else {
            console.error('User not logged in')
        }
    } else {
        console.error('DB not connected!')
    }
}
```

### ✔️ **Guard Clause Technique**

```javascript
function foo() {
    if (!dbConnected) {
        console.error('DB not connected!')
        return
    }

    if (!userLogged) {
        console.error('User not logged in')
        return
    }

    if (!isAdmin) {
        console.error('Only for admins!')
        return
    }

    console.log('Success!')
}
```