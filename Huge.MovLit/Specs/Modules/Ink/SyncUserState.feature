Feature: Sync User State
  As the Ink module
  I want to synchronize Ink variables with user settings
  So that story progress persists across sessions

  Scenario: Load user state into Ink variables
    Given a compiled Ink story has variables
    And the user settings contain matching "ink:" prefixed keys
    When user state is synced
    Then the Ink variables should be updated from user settings

  Scenario: Observe Ink variable changes
    Given a compiled Ink story has variables
    When an Ink variable value changes during story execution
    Then the new value should be saved to user settings with the "ink:" prefix
    And the SiteState InkVariable property should be notified

  Scenario: React to external user state changes
    Given the Ink module is listening to SiteState property changes
    When the UserState property changes
    Then the Ink variables should be re-synced from user settings
