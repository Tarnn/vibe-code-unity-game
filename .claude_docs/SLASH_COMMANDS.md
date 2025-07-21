# Custom Slash Commands for FrostRealm Chronicles Development

## Overview
This document defines custom slash commands for Claude to streamline development workflows for FrostRealm Chronicles. These commands provide quick access to common development tasks with pre-configured prompts and context.

## Usage
In Claude or Cursor, use these slash commands by typing `/command_name` followed by any arguments. Each command expands to a full prompt with relevant context from your project documentation.

## Core Development Commands

### /implement
**Purpose**: Generate implementation for game systems
**Usage**: `/implement [system_name] [additional_context]`

**Expanded Prompt**:
```
Implement [system_name] for FrostRealm Chronicles using Unity 6.1 ECS/DOTS.

Project Context:
- RTS game inspired by Warcraft III: The Frozen Throne
- Target: 60 FPS with 500+ units
- Unity 6.1 LTS with HDRP rendering
- ECS/DOTS for performance-critical systems

Requirements from GDD/TechSpecs:
[Auto-include relevant sections based on system_name]

Implementation Requirements:
- Follow TFT mechanics exactly where applicable
- Use Burst compilation for performance
- Include comprehensive XML documentation
- Add error handling and validation
- Follow FrostRealm.Core namespace conventions

Output: Complete C# implementation with integration notes
```

### /debug
**Purpose**: Debug Unity/RTS specific issues
**Usage**: `/debug [issue_description]`

**Expanded Prompt**:
```
Debug this Unity RTS issue in FrostRealm Chronicles:

Issue: [issue_description]
Expected Behavior: [What should happen based on TFT mechanics]
Actual Behavior: [Current problematic behavior]

System Context: Unity 6.1 LTS, ECS/DOTS, HDRP
Performance Target: 60 FPS with 500+ units

Analysis Required:
- Root cause identification
- TFT mechanics validation
- Performance impact assessment
- Fix implementation
- Prevention strategies

Reference AI_DEBUGGING_GUIDE.md for common patterns
```

### /optimize
**Purpose**: Performance optimization for RTS systems
**Usage**: `/optimize [system_or_code]`

**Expanded Prompt**:
```
Optimize this code for FrostRealm Chronicles RTS performance:

Target Performance: 60 FPS with 500+ units
Current Performance: [To be measured]

Optimization Focus:
- ECS/DOTS conversion where applicable
- Burst compilation implementation
- Memory allocation reduction
- Job system parallelization
- Cache-friendly data access patterns

Code to Optimize:
[system_or_code]

Requirements:
- Maintain exact TFT mechanics
- Preserve functionality
- Add profiler markers
- Include performance measurements

Reference AI_PERFORMANCE_GUIDE.md for optimization patterns
```

### /test
**Purpose**: Generate comprehensive tests
**Usage**: `/test [system_name] [test_type]`

**Expanded Prompt**:
```
Generate Unity Test Framework tests for [system_name] in FrostRealm Chronicles:

Test Type: [test_type] (unit/integration/performance)

Test Requirements:
- TFT mechanics accuracy validation
- Edge case coverage
- Performance benchmarking (if applicable)
- ECS system testing patterns
- Mock system integration

Test Categories:
- Formula accuracy (±0.1 tolerance for TFT formulas)
- Component behavior validation
- System integration testing
- Performance regression detection

Output: Complete test implementation with AAA pattern
Reference AI_TESTING_FRAMEWORK.md for patterns
```

### /review
**Purpose**: Code review and quality assurance
**Usage**: `/review [code_to_review]`

**Expanded Prompt**:
```
Review this FrostRealm Chronicles code for quality and compliance:

Code to Review:
[code_to_review]

Review Criteria:
- TFT mechanics accuracy
- Unity 6.1 best practices
- ECS/DOTS patterns
- Performance considerations
- Code quality and maintainability
- Security and error handling

Review Standards:
- Namespace: FrostRealm.*
- Documentation: XML comments required
- Performance: Burst-compatible where possible
- Testing: Unit test coverage expectations

Reference AI_CODE_REVIEW.md for quality standards
```

## Asset Creation Commands

### /asset
**Purpose**: Generate AI asset creation prompts
**Usage**: `/asset [asset_type] [faction] [description]`

**Expanded Prompt**:
```
Generate AI asset creation prompt for FrostRealm Chronicles:

Asset Type: [asset_type] (model/texture/animation/audio)
Faction: [faction] (Human/Orc/Undead/NightElf/Neutral)
Description: [description]

Style Requirements:
- Warcraft III: Reforged quality and fidelity
- Faction-appropriate color palette
- Isometric RTS optimization
- 4K texture resolution for primary assets
- High-poly detail with LOD considerations

Technical Specifications:
- Unity 6.1 HDRP compatibility
- PBR material workflow
- Performance targets for RTS camera
- Integration with existing asset pipeline

Output: Optimized prompts for AI generation tools
Reference AI_ASSET_PIPELINE.md for specifications
```

### /art
**Purpose**: Generate concept art and visual direction
**Usage**: `/art [concept_description] [style_notes]`

**Expanded Prompt**:
```
Generate concept art prompt for FrostRealm Chronicles:

Concept: [concept_description]
Style Notes: [style_notes]

Art Direction:
- Warcraft III: Reforged inspired aesthetic
- Stylized realism with fantasy elements
- High contrast for RTS readability
- Faction-appropriate visual language

Technical Requirements:
- Suitable for 3D model reference
- Clear silhouette definition
- Material and lighting reference
- Proportions optimized for isometric view

Mood and Atmosphere:
- Epic fantasy setting
- Frozen realm theme
- Heroic and dramatic tone

Output: Detailed concept art generation prompt
```

## Specialized Workflow Commands

### /tft
**Purpose**: Validate TFT mechanics implementation
**Usage**: `/tft [mechanic_name] [implementation]`

**Expanded Prompt**:
```
Validate TFT mechanics implementation for [mechanic_name]:

Current Implementation:
[implementation]

TFT Reference Data:
[Auto-include relevant formulas from GDD/TechSpecs]

Validation Requirements:
- Formula accuracy (exact TFT calculations)
- Edge case behavior matching
- Type effectiveness tables
- Hero attribute scaling
- Resource management rules

Expected Tolerances:
- Floating point: ±0.1
- Integer calculations: Exact match
- Percentage calculations: ±0.01%

Output: Validation report with corrections if needed
```

### /balance
**Purpose**: Game balance analysis and tuning
**Usage**: `/balance [system_or_unit] [balance_concern]`

**Expanded Prompt**:
```
Analyze game balance for FrostRealm Chronicles:

System/Unit: [system_or_unit]
Balance Concern: [balance_concern]

Balance Framework:
- TFT reference values as baseline
- Asymmetric faction design principles
- Rock-paper-scissors unit counters
- Economic balance considerations

Analysis Areas:
- Power level relative to cost
- Counter-play opportunities
- Timing windows and tech dependencies
- Multiplayer implications

Metrics to Consider:
- Win rates by faction
- Unit usage statistics
- Average game duration
- Strategic diversity

Output: Balance analysis with tuning recommendations
```

### /netcode
**Purpose**: Multiplayer networking implementation
**Usage**: `/netcode [feature_description]`

**Expanded Prompt**:
```
Implement multiplayer networking for FrostRealm Chronicles:

Feature: [feature_description]

Networking Framework:
- Unity Netcode for GameObjects/Entities
- Client-server architecture
- Authoritative server for game state
- Client prediction for responsiveness

RTS-Specific Requirements:
- Deterministic simulation
- Command queuing and validation
- State synchronization
- Lag compensation strategies
- Anti-cheat considerations

Performance Targets:
- <100ms latency impact
- Support for 4+ players
- Bandwidth optimization
- Graceful degradation

Output: Complete networking implementation
```

## Documentation Commands

### /docs
**Purpose**: Generate or update documentation
**Usage**: `/docs [documentation_type] [content_area]`

**Expanded Prompt**:
```
Generate documentation for FrostRealm Chronicles:

Documentation Type: [documentation_type] (API/user/design)
Content Area: [content_area]

Documentation Standards:
- Clear, concise writing
- Code examples where applicable
- Integration with existing docs
- Markdown formatting
- Cross-references to related systems

Structure Requirements:
- Overview and purpose
- Usage examples
- API reference (if applicable)
- Best practices
- Troubleshooting guide

Output: Complete documentation section
```

### /gdd
**Purpose**: Update Game Design Document
**Usage**: `/gdd [section] [updates]`

**Expanded Prompt**:
```
Update Game Design Document section for FrostRealm Chronicles:

Section: [section]
Updates: [updates]

GDD Standards:
- Maintain TFT mechanical fidelity
- Include exact formulas and values
- Reference Unity 6.1 implementation details
- Balance and design rationale
- Technical feasibility notes

Integration Requirements:
- Consistency with existing GDD sections
- Cross-reference to TechSpecs where applicable
- Update dependent sections if needed
- Maintain version history

Output: Updated GDD section with change notes
```

## Quality Assurance Commands

### /qa
**Purpose**: Quality assurance and testing protocols
**Usage**: `/qa [test_scenario] [quality_criteria]`

**Expanded Prompt**:
```
Generate QA protocol for FrostRealm Chronicles:

Test Scenario: [test_scenario]
Quality Criteria: [quality_criteria]

QA Framework:
- Functional testing protocols
- Performance validation
- TFT mechanics verification
- User experience evaluation
- Regression testing procedures

Test Coverage:
- Core gameplay mechanics
- Edge cases and error conditions
- Performance under load
- Cross-platform compatibility
- Accessibility compliance

Output: Comprehensive QA test plan
```

### /ci
**Purpose**: Continuous integration setup
**Usage**: `/ci [pipeline_component]`

**Expanded Prompt**:
```
Setup CI/CD pipeline component for FrostRealm Chronicles:

Component: [pipeline_component]

Pipeline Requirements:
- Unity 6.1 build automation
- Automated testing execution
- Performance regression detection
- Asset validation
- Quality gate enforcement

Integration Points:
- Git repository hooks
- Unity Cloud Build
- Testing framework execution
- Deployment automation
- Monitoring and alerting

Output: CI/CD configuration and scripts
```

## Usage Examples

### Basic Implementation
```
/implement ResourceSystem
```
Generates a complete resource management system implementation.

### Performance Optimization
```
/optimize CombatSystem "experiencing frame drops during large battles"
```
Analyzes and optimizes combat system for better performance.

### Asset Creation
```
/asset model Orc "Grunt warrior with battle axe and tribal armor"
```
Generates AI prompts for creating an Orc Grunt 3D model.

### TFT Validation
```
/tft damage_calculation "current implementation seems to give wrong values"
```
Validates damage calculation against TFT formulas.

### Testing
```
/test HealthComponent unit
```
Generates unit tests for the HealthComponent system.

## Command Chaining
Commands can be chained for complex workflows:

```
/implement MovementSystem
/test MovementSystem unit
/optimize MovementSystem
/review [generated_code]
```

This creates an implementation, tests it, optimizes it, and reviews the final result.

## Customization
Add new slash commands by following this pattern:

1. Define command purpose and usage
2. Create expanded prompt template
3. Include relevant project context
4. Reference appropriate documentation
5. Specify expected output format

These slash commands streamline development by providing consistent, well-contextualized prompts for common tasks.