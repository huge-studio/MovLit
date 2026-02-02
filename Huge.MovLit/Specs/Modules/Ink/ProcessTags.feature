Feature: Process Story Tags
  As the Ink module
  I want to process inline tags in the story content
  So that media elements are displayed alongside the narrative

  Scenario: Process a Lottie tag
    Given the story produces tags containing "lottie:animation.json"
    When the tags are processed
    Then the SiteState Lottie property should be set to the animation URL

  Scenario: Process an image tag
    Given the story produces tags containing "image:photo.jpg"
    When the tags are processed
    Then the SiteState Image property should be set to the image URL

  Scenario: Process both Lottie and image tags
    Given the story produces tags containing both "lottie:anim.json" and "image:fallback.jpg"
    When the tags are processed
    Then the SiteState Lottie property should be set
    And the SiteState Image property should be set

  Scenario: No media tags present
    Given the story produces tags with no media references
    When the tags are processed
    Then the SiteState media properties should remain unchanged
