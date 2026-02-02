Feature: Audio Playback
  As a user viewing a story
  I want to control audio playback
  So that I can listen to background music or sound effects

  Scenario: Play audio track
    Given the controls module is loaded
    And a track URL is configured in settings
    And audio is not currently playing
    When I click the play button
    Then the audio should start playing

  Scenario: Pause audio track
    Given the controls module is loaded
    And audio is currently playing
    When I click the pause button
    Then the audio should stop playing

  Scenario: Toggle mute on
    Given the controls module is loaded
    And audio is not muted
    When I click the mute toggle button
    Then the audio should be muted

  Scenario: Toggle mute off
    Given the controls module is loaded
    And audio is muted
    When I click the mute toggle button
    Then the audio should be unmuted

  Scenario: Auto-play when SiteState sound URL changes
    Given the controls module is loaded
    And the page is not in edit mode
    When the SiteState sound URL property changes to a valid URL
    Then the current track should update to the new URL
    And the audio should start playing

  Scenario: Ignore SiteState sound URL changes in edit mode
    Given the controls module is loaded
    And the page is in edit mode
    When the SiteState sound URL property changes
    Then the audio should not start playing

  Scenario: Audio loops by default
    Given the controls module is loaded
    And a track URL is configured in settings
    When the audio begins playing
    Then the audio should be set to loop
