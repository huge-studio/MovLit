Feature: Load Dashboard
  As a user visiting the site
  I want to see the dashboard with configured sections
  So that I can discover and browse stories

  Scenario: Load dashboard with configured sections
    Given the dashboard module has sections configured
    When the dashboard loads
    Then the configured content sections should be displayed
    And the hero section visibility should match the settings

  Scenario: Load dashboard with hero enabled
    Given the dashboard settings have ShowHero enabled
    And a featured story is configured
    When the dashboard loads
    Then the hero banner should be displayed
    And the featured story details should be shown

  Scenario: Load dashboard with hero disabled
    Given the dashboard settings have ShowHero disabled
    When the dashboard loads
    Then the hero banner should not be displayed

  Scenario: Detect browse mode from query string
    Given the URL contains "browse=true"
    When the dashboard loads
    Then the dashboard should display the browse view
    And the category should be read from the query string
