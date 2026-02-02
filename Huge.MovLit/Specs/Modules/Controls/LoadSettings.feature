Feature: Load Controls Settings
  As an administrator
  I want to configure the controls module settings
  So that I can set the default audio track

  Scenario: Load track URL from module settings
    Given the controls module has settings configured
    When the controls module loads
    Then the track URL should be loaded from the "TrackUrl" setting

  Scenario: Load with no track URL configured
    Given the controls module has no track URL setting
    When the controls module loads
    Then the track URL should be empty
    And the audio player should not auto-play

  Scenario: Update track URL setting
    Given I am on the controls settings page
    When I enter a new track URL
    And I save the settings
    Then the "TrackUrl" setting should be updated
