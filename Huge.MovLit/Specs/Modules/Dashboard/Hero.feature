Feature: Hero Banner
  As a user visiting the dashboard
  I want to see a featured story in a hero banner
  So that I can quickly access the highlighted story

  Scenario: Display hero with featured story
    Given the dashboard settings have a featured story configured
    And ShowHero is enabled
    When the hero component loads
    Then the featured story details should be displayed in the hero banner

  Scenario: Navigate to featured story
    Given the hero banner is displaying a featured story
    When I click on the hero banner
    Then I should be navigated to the featured story page

  Scenario: Hide hero when disabled
    Given ShowHero is disabled in the settings
    When the hero component loads
    Then the hero banner should not be rendered

  Scenario: Handle missing featured story gracefully
    Given the dashboard settings have a featured story ID
    But the story no longer exists
    When the hero component loads
    Then the hero banner should not display a story
