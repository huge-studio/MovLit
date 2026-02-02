Feature: Lottie Module Settings
  As an administrator
  I want to configure the Lottie module
  So that I can customize the animation display behavior

  Scenario: Load Lottie settings
    Given I am on the Lottie settings page
    When the settings load
    Then the module settings should be displayed

  Scenario: Update Lottie settings
    Given I am editing Lottie settings
    When I save the settings
    Then the module settings should be persisted
