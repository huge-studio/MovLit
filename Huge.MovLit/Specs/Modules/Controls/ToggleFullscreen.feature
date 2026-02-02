Feature: Toggle Fullscreen
  As a user viewing a story
  I want to toggle fullscreen mode
  So that I can have an immersive reading experience

  Scenario: Enter fullscreen mode
    Given the controls module is loaded
    And the page is not in fullscreen mode
    When I click the fullscreen toggle button
    Then the page should enter fullscreen mode
    And the URL should contain "fullscreen=true"

  Scenario: Exit fullscreen mode
    Given the controls module is loaded
    And the page is in fullscreen mode
    When I click the fullscreen toggle button
    Then the page should exit fullscreen mode
    And the URL should contain "fullscreen=false"

  Scenario: Load fullscreen state from URL
    Given the URL contains "fullscreen=true"
    When the controls module loads
    Then the page should be in fullscreen mode

  Scenario: Load without fullscreen parameter
    Given the URL does not contain a fullscreen parameter
    When the controls module loads
    Then the page should not be in fullscreen mode
