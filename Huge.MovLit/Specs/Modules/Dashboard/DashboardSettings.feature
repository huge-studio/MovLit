Feature: Dashboard Settings
  As an administrator
  I want to configure the dashboard module
  So that I can control which sections, links, and tags are displayed

  Scenario: Load dashboard settings
    Given I am on the dashboard settings page
    When the settings load
    Then the configured sections should be displayed
    And the quick links should be displayed
    And the popular tags should be displayed

  Scenario: Add a new content section
    Given I am editing dashboard settings
    When I click add section
    Then a new empty section should be added to the sections list

  Scenario: Remove a content section
    Given I am editing dashboard settings with 3 sections
    When I remove the second section
    Then there should be 2 sections remaining

  Scenario: Reorder sections - move up
    Given I am editing dashboard settings with sections "A", "B", "C"
    When I move section "B" up
    Then the section order should be "B", "A", "C"

  Scenario: Reorder sections - move down
    Given I am editing dashboard settings with sections "A", "B", "C"
    When I move section "A" down
    Then the section order should be "B", "A", "C"

  Scenario: Add a quick link
    Given I am editing dashboard settings
    When I click add quick link
    Then a new empty quick link should be added

  Scenario: Remove a quick link
    Given I am editing dashboard settings with 2 quick links
    When I remove the first quick link
    Then there should be 1 quick link remaining

  Scenario: Reorder quick links - move up
    Given I am editing dashboard settings with quick links "Home", "About", "Contact"
    When I move quick link "About" up
    Then the quick link order should be "About", "Home", "Contact"

  Scenario: Reorder quick links - move down
    Given I am editing dashboard settings with quick links "Home", "About", "Contact"
    When I move quick link "Home" down
    Then the quick link order should be "About", "Home", "Contact"

  Scenario: Set quick link page auto-populates URL
    Given I am editing a quick link
    When I select a page from the page dropdown
    Then the quick link URL should be set to the page path
    And the quick link name should default to the page name if not set

  Scenario: Toggle a popular tag on
    Given I am editing dashboard settings
    When I toggle on the tag "adventure"
    Then "adventure" should be in the popular tags list

  Scenario: Toggle a popular tag off
    Given I am editing dashboard settings with tag "adventure" enabled
    When I toggle off the tag "adventure"
    Then "adventure" should not be in the popular tags list

  Scenario: Save dashboard settings
    Given I have made changes to dashboard settings
    When I save the settings
    Then the module settings should be persisted
