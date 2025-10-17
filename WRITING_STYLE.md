# Writing Style Guide

This document explains the writing style used in the Daisy-m4 project and how you can customize it to fit your needs.

## What is the Writing Style?

The Daisy-m4 project uses a structured, technical writing style that emphasizes:

### 1. **Clarity and Precision**
- Documentation is concise but comprehensive
- Technical terms are used consistently
- Each module and concept is clearly defined

### 2. **Structured Documentation**
The project follows a layered documentation approach:

- **README.md**: High-level overview, quick start guide, and testing information
- **AGENTS.md**: Guidelines for AI agents and developers, including coding standards and testing requirements
- **instructions/**: Detailed technical documentation
  - `workflows.md`: Workflow specifications and module sequences
  - `engine_design.md`: Orchestration design and traversal mechanics
  - `plugin_injection.md`: Plugin system and factory patterns

### 3. **Code Documentation Style**
- Code follows .NET naming conventions
- Comments explain "why" rather than "what" when the code is self-explanatory
- Each module includes appropriate inline comments for complex logic
- Public APIs are documented with XML documentation comments

### 4. **AI Agent Instructions**
The writing style for AI agent interactions is defined in `AGENTS.md` and emphasizes:
- Professional, technical tone
- Step-by-step implementation guidance
- Mandatory testing and documentation requirements
- Code formatting compliance

## How to Change the Writing Style

You can customize the writing style for your project by modifying these files:

### 1. **For AI Agents and Automated Tools**

Edit `.github/copilot-instructions.md` and `AGENTS.md`:

```markdown
# AGENTS.md

## Identity and Goal
You are an experienced [YOUR ROLE HERE] with knowledge of [YOUR TECH STACK].
Your goal is to [YOUR GOALS HERE].

## Details on the solution
[Your solution description and patterns]

## Rules
- [Your custom rules here]
```

**Key sections to customize:**
- **Identity and Goal**: Define the role and expertise level
- **Details on the solution**: Explain your architecture and patterns
- **Rules**: Set your coding standards and conventions
- **Testing Requirements**: Define your test coverage expectations
- **Documentation Requirements**: Specify what documentation is mandatory
- **Code Formatting Requirements**: Set your formatting standards

### 2. **For Project Documentation**

Modify the documentation files to match your preferred style:

**README.md**: Update the tone and structure to match your audience
- For technical audiences: Keep it concise and focused on API/architecture
- For general audiences: Add more context and examples
- For beginners: Include more step-by-step instructions

**instructions/**: Adjust the technical depth
- More abstract: Focus on concepts and design patterns
- More concrete: Add more code examples and implementation details

### 3. **For Code Comments and Documentation**

Set your standards in `AGENTS.md` under the "Rules" section:

```markdown
## Code Documentation Standards
- Use XML documentation for all public APIs
- Add inline comments only for complex business logic
- Follow [Microsoft's C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
```

### 4. **For Workflow Descriptions**

Edit `instructions/workflows.md` to match your documentation style:

**Current style**: Step-by-step with numbered lists and module names
```markdown
1. Receive external input via System.Console
   - Daisy.Receivers.Console
2. Process the input
   - Daisy.Abilities.Operator
```

**Alternative style**: Descriptive paragraphs
```markdown
The workflow begins by receiving external input through the Console receiver,
which initializes the Impulse object. The Operator ability then processes...
```

## Examples of Different Writing Styles

### Technical/Academic Style
- Emphasizes precision and formal language
- Uses passive voice and technical terminology
- Includes references and citations

### Casual/Tutorial Style
- Uses active voice and direct address ("you")
- Includes examples and analogies
- Explains concepts before diving into details

### Reference/API Style
- Lists features and parameters
- Minimal prose, maximum information density
- Focuses on "what" rather than "why"

## Current Style Characteristics

The Daisy-m4 project currently uses a **Technical-Professional** style:

| Characteristic | Current Approach |
|----------------|------------------|
| **Tone** | Professional, direct |
| **Voice** | Mix of active and passive |
| **Audience** | Experienced .NET developers |
| **Detail Level** | High for architecture, moderate for usage |
| **Examples** | Code-focused, minimal |
| **Structure** | Hierarchical with clear sections |

## Recommendations for Customization

1. **Identify your primary audience**: Who will be using your fork?
   - Students/Beginners: Add more explanations and examples
   - Enterprise developers: Focus on scalability and best practices
   - Researchers: Add more theory and design rationale

2. **Decide on your documentation depth**:
   - Minimal: Only essential information
   - Standard: Current level
   - Comprehensive: Include tutorials, examples, and troubleshooting

3. **Choose your code comment style**:
   - Self-documenting: Minimal comments, clear naming
   - Explanatory: Comments explain the reasoning
   - Educational: Comments teach concepts

4. **Set your AI agent personality** (in AGENTS.md):
   - Strict/Formal: Enforce all rules rigidly
   - Flexible/Pragmatic: Allow judgment calls
   - Educational: Explain decisions and alternatives

## Making Changes

To implement your writing style changes:

1. **Fork the repository** or create a branch
2. **Update AGENTS.md** with your AI agent instructions
3. **Update documentation files** (README.md, instructions/*.md)
4. **Create a style guide** (like this one) for contributors
5. **Update .editorconfig** for code formatting preferences
6. **Document your changes** in your commit messages

## Validation

After changing the writing style, ensure:
- [ ] All documentation uses consistent terminology
- [ ] Code examples match your new standards
- [ ] AI agents (if used) generate code in the expected style
- [ ] Contributors understand the new style expectations
- [ ] Documentation is still clear and accessible to your target audience

## Resources

- [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [.NET API Documentation Guidelines](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/)
- [GitHub Documentation Best Practices](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features)
- [Technical Writing Guidelines](https://developers.google.com/tech-writing)
