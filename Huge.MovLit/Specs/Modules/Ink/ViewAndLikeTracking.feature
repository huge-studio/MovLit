Feature: View and Like Tracking
  As a content platform
  I want to track views and likes on stories
  So that authors and readers can see engagement metrics

  Scenario: Log a view on first render
    Given a story is loaded with a valid StoryId
    And the user has a valid visitor ID or user ID
    When the module renders for the first time
    Then a view should be logged for the story

  Scenario: Do not log duplicate views
    Given a view has already been logged for this session
    When the module re-renders
    Then no additional view should be logged

  Scenario: Do not log view for anonymous users without visitor ID
    Given a story is loaded
    And the user has no visitor ID and no user ID
    When the module renders for the first time
    Then no view should be logged

  Scenario: Load story metrics
    Given a story is loaded with a valid StoryId
    When metrics are loaded
    Then the view count should be populated
    And the like count should be populated
    And the current user's like status should be determined

  Scenario: Like a story
    Given the user can like stories
    And the story is not currently liked by the user
    When I click the like button
    Then the story should be liked
    And the like count should increase by 1

  Scenario: Unlike a story
    Given the user can like stories
    And the story is currently liked by the user
    When I click the like button
    Then the story should be unliked
    And the like count should decrease by 1

  Scenario: Disable like for users without identity
    Given the user has no visitor ID and no user ID
    Then the like button should be disabled
