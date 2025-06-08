# Clarification for Unit Tests (Authentication)
---

### Unit Test 1 (Register_NewEmail_ReturnsOkWithToken)
- Creates an empty in-memory DB.
- Calls POST `/api/auth/register` (via the controller) with a brand-new e-mail.
- Expects `200 OK` and that the JSON payload contains the same e-mail and a JWT string `("fakeToken")`

Why it matters?
Confirms the happy-path: a new user is persisted and the service issues a token.

### Unit Test 2 (Register_DuplicateEmail_ReturnConflict)
- Seeds a user with `dupe@ex.com`.
- Tries to register the same e-mail again.
- Expects `409 Conflict`

Why it matters?
Enforces the rule `"e-mail must be unique"`. Prevents regression that could re-introduce duplicate-user bugs.

### Unit Test 3 (Login_CorrectCreds_ReturnsOkWithToken)
- Seeds a user whose password hash matches `"secret"`.
- Calls `login` with the right password.
- Expects `200 OK` and gets a token.

Why it matters?
Confirms the hashing + verification logic and success response.

### Unit Test 4 (Login_WrongPassword_ReturnsConflict)
- Same as `Unit Test 3` but passes `"wrong"` password.
- Expects `409 Conflict` (Auth failure)

Why it matters?
Makes sure invalid creds don't accidentally log a user in.

### Unit Test 5 (Register_WeakPassword_ReturnBadRequest)
- Calls `POST /api/auth/register` with a valid email but a weak password `"weakpass1"`
- Manually triggers validation and injects model errors into the controller
- Expects `400 Bad Request` and a message that includes `"password"`

Why it matters?
Enforces password strength rules, ensuring weak passwords are rejected before user creation.
This helps maintain security and avoid poor credential practices.
