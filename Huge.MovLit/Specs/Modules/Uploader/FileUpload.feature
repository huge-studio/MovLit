Feature: File Upload
  As an administrator
  I want to upload files through the uploader module
  So that I can add media assets to the site

  Scenario: Display uploader module
    Given the uploader module is loaded on a page
    When the module renders
    Then the uploader interface should be displayed
    And the settings link should be available

  Scenario: Upload a file successfully
    Given I am on the uploader settings page
    When I upload a valid file
    Then the file should be stored via the FileService
    And the file path should be set in the settings view model

  Scenario: Handle upload failure
    Given I am on the uploader settings page
    When the file upload fails
    Then an error message should be displayed
