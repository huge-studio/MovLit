Feature: Browse Stories
  As a user
  I want to browse and filter stories
  So that I can find stories that interest me

  Scenario: Load stories on initialization
    Given the browse component is loaded
    When the stories are fetched from the service
    Then the story list should be populated
    And the first page of stories should be displayed

  Scenario: Filter stories by category - Fresh
    Given the browse component has stories loaded
    When I select the "Fresh" category
    Then stories should be sorted by creation date descending

  Scenario: Filter stories by category - Top
    Given the browse component has stories loaded
    When I select the "Top" category
    Then stories should be sorted by upvote count descending

  Scenario: Filter stories by category - Trending
    Given the browse component has stories loaded
    When I select the "Trending" category
    Then only stories from the last 30 days should be shown
    And they should be sorted by upvote count then creation date

  Scenario: Filter stories by tag
    Given the browse component has stories loaded
    And the mode is set to "tag"
    When I select the tag "adventure"
    Then only stories tagged with "adventure" should be displayed

  Scenario: Search stories by title
    Given the browse component has stories loaded
    When I enter "dragon" in the search field
    Then only stories with "dragon" in the title or description should be displayed

  Scenario: Search stories by tag text
    Given the browse component has stories loaded
    When I enter "mystery" in the search field
    Then stories with "mystery" in their tags should be included

  Scenario: Clear search
    Given the browse component has an active search term
    When I clear the search
    Then all stories in the current filter should be displayed

  Scenario: Navigate to next page
    Given the browse component has more than one page of stories
    And I am on the first page
    When I click next page
    Then the next set of stories should be displayed

  Scenario: Navigate to previous page
    Given the browse component is on page 2
    When I click previous page
    Then the first page of stories should be displayed

  Scenario: Navigate to a story
    Given the browse component has stories loaded
    When I click on a story card
    Then I should be navigated to the story page

  Scenario: Switch browse mode from category to tag
    Given the browse mode is "category"
    When I change the mode to "tag"
    Then the filter should switch to tag-based filtering
    And the page should reset to page 1

  Scenario: External tag change updates browse
    Given the browse mode is "tag"
    When an external tag filter event fires with tag "horror"
    Then the browse list should filter to stories tagged "horror"

  Scenario: Go back to dashboard
    Given I am on the browse view
    When I click the back button
    Then I should be navigated back to the dashboard
