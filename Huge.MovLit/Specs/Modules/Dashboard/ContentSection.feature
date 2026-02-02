Feature: Content Section
  As a user viewing the dashboard
  I want to see story cards grouped in content sections
  So that I can discover stories by category

  Scenario: Load stories for a content section
    Given a content section is configured with title "New Stories"
    When the section loads
    Then stories should be fetched for the "New Stories" category
    And story metrics should be loaded for the displayed stories

  Scenario: Display story cards with metrics
    Given a content section has loaded stories with metrics
    Then each story card should display the story title
    And each story card should show view and like counts

  Scenario: Navigate to a story from content section
    Given a content section has loaded stories
    When I click on a story card
    Then I should be navigated to the story page

  Scenario: Navigate to browse view from content section
    Given a content section with title "Top Stories"
    When I click the "View More" action link
    Then I should be navigated to the browse view
    And the category should be set to "top-stories"

  Scenario: Load section with no stories
    Given a content section is configured
    And no stories exist for the category
    When the section loads
    Then the section should display with an empty story list
