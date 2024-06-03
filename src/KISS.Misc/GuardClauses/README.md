## Usage

```javascript

// 🔥 NESTING HELL
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

// ✔️ GUARD CLAUSE TECHNIQUE
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

## Acknowledgements

- [Replace Nested Conditional with Guard Clauses](https://refactoring.guru/replace-nested-conditional-with-guard-clauses)