# Important Notes Regarding Database Testing
- When running the Integration tests and it involves the database, please **run each test classes separately** to avoid race conditions for now.
  - **Running all unit tests at once** will generate errors due to timings.