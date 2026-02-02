Feature: Ink External Functions
  As a story author using Ink
  I want to call external functions from my story scripts
  So that I can trigger media, navigation, and inventory actions

  Scenario: Play a sound from Ink script
    Given an Ink story is running with bound external functions
    When the story calls playSound with a sound URL
    Then the SiteState SoundUrl property should be updated

  Scenario: Show an image from Ink script
    Given an Ink story is running with bound external functions
    When the story calls showImage with an image URL
    Then the SiteState Image property should be updated

  Scenario: Show a Lottie animation from Ink script
    Given an Ink story is running with bound external functions
    When the story calls showLottie with a Lottie URL
    Then the SiteState Lottie property should be updated

  Scenario: Navigate to a URL from Ink script
    Given an Ink story is running with bound external functions
    When the story calls navigateUrl with a target URL
    Then the browser should navigate to the specified URL

  Scenario: Navigate to a URL with hash anchor
    Given an Ink story is running with bound external functions
    When the story calls navigateUrl with "#"
    Then the current page should be reloaded

  Scenario: Navigate to a page by name from Ink script
    Given an Ink story is running with bound external functions
    When the story calls navigatePage with a page name
    Then the browser should navigate to the named page

  Scenario: Register an inventory item from Ink script
    Given an Ink story is running with bound external functions
    When the story calls registerItem with name "sword", type "weapon", and icon "sword.png"
    Then the item should be registered in the InventoryService

  Scenario: Collect an inventory item from Ink script
    Given an Ink story is running with bound external functions
    And an item "sword" is registered
    When the story calls collectItem with "sword"
    Then the item should be marked as collected in the InventoryService

  Scenario: Check if user has an inventory item
    Given an Ink story is running with bound external functions
    And the user has collected item "sword"
    When the story calls hasItem with "sword"
    Then the function should return true

  Scenario: Check for item user does not have
    Given an Ink story is running with bound external functions
    And the user has not collected item "shield"
    When the story calls hasItem with "shield"
    Then the function should return false

  Scenario: Count collected items by type
    Given an Ink story is running with bound external functions
    And the user has collected 3 items of type "weapon"
    When the story calls countItems with "weapon"
    Then the function should return 3

  Scenario: Get total registered items by type
    Given an Ink story is running with bound external functions
    And 5 items of type "weapon" are registered
    When the story calls totalItems with "weapon"
    Then the function should return 5

  Scenario: Get item icon
    Given an Ink story is running with bound external functions
    And an item "sword" is registered with icon "sword.png"
    When the story calls itemIcon with "sword"
    Then the function should return "sword.png"
