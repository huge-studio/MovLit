Feature: Uploader Settings
  As an administrator
  I want to configure the uploader module settings
  So that I can manage the uploaded file path

  Scenario: Load uploader settings
    Given I am on the uploader settings page
    When the settings load
    Then the current file path should be displayed

  Scenario: Save uploader settings
    Given I am editing uploader settings
    When I save the settings
    Then the file path setting should be persisted
