Feature: Display Lottie Animation
  As a user reading a story
  I want to see Lottie animations and images alongside the narrative
  So that the story experience is visually enriched

  Scenario: Display Lottie animation from SiteState
    Given the Lottie module is loaded
    And the SiteState has a Lottie URL set
    When the module renders
    Then the Lottie animation should be displayed

  Scenario: Display fallback image from SiteState
    Given the Lottie module is loaded
    And the SiteState has an Image URL set
    And no Lottie URL is set
    When the module renders
    Then the fallback image should be displayed

  Scenario: Update animation when Lottie property changes
    Given the Lottie module is loaded
    And the page is not in edit mode
    When the SiteState Lottie property changes to a new URL
    Then the Lottie animation should update to the new source
    And the image source should be cleared
    And the animation should auto-play

  Scenario: Update image when Image property changes
    Given the Lottie module is loaded
    And the page is not in edit mode
    When the SiteState Image property changes to a new URL
    Then the image should update to the new source
    And the Lottie source should be cleared

  Scenario: Ignore property changes in edit mode
    Given the Lottie module is loaded
    And the page is in edit mode
    When the SiteState Lottie property changes
    Then the display should not update

  Scenario: Display nothing when no media is set
    Given the Lottie module is loaded
    And the SiteState has no Lottie URL or Image URL
    When the module renders
    Then no media content should be displayed
