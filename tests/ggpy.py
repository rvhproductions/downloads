import random
import os
build = "v1.3b1_27_1_21"
version = "1.3 Beta 1"
beta = "true"
if beta == "true":
    print("[BETA VERSION - " + build + "]")
    print("Running on: " + os.name)
    print("This build may have bugs, report them at an RVH Productions Team Member!")
mode = 0
print("")
print("")
print("GuessGame for Python")
print("")
print("Version: " + version)
print("")
print("Select an option to continue!")
print("")
print("Start playing a game mode:")
print("1. Easy")
print("2. Normal")
print("3. Hard")
print("4. Very Hard")
print("5. Professional")
print("6. God Mode")
print("")
print("Other options:")
print("7. Credits")
print("8. Game info")
if beta == "true":
    print("9. Report a bug")
while mode == 0:
    option = int(input("Option: "))
    if option == 1:
        mode = 1
    elif option == 2:
        mode = 2
    elif option == 3:
        mode = 3
    elif option == 4:
        mode = 4
    elif option == 5:
        mode = 5
    elif option == 6:
        mode = 6
    elif option == 7:
        print("Credits")
        print("")
        print("Developed and published by RVH Productions.")
        print("")
        print("Copyright RVH Productions.")
        mode = 0
    elif option == 8:
        print("Game info")
        print("")
        print("GuessGame for Python " + version)
        print("Version build name: " + build)
        mode = 0
    elif option == 9:
        if beta == "true":
            print("If you want to report a bug, you can contact someone from the RVH Productions Team.")
            print("")
            print("Reporting a bug is simple, contact a Team Member on our Discord server, or via email.")
            print("")
            print("When asked about the version, make sure to include this: " + build)
        mode = 0
    elif option == 10:
        mode = 10
    else:
        print("Invalid option, please choose again.")
        mode = 0
if mode == 1:
    number = random.randint(0,100)
elif mode == 2:
    number = random.randint(0,500)
elif mode == 3:
    number = random.randint(0,1000)
elif mode == 4:
    number = random.randint(0,10000)
elif mode == 5:
    number = random.randint(0,100000)
elif mode == 6:
    number = random.randint(0,1000000)
elif mode == 10:
    number = random.randint(0,10000000000)
    print("You found the secret mode!")
    print("")
    print("Warning:")
    print("")
    print("This secret mode is very hard.")
    print("It may take a very long time to guess the number.")
    print("")
print("")
print("Guess the number!")
print("")
guessloop = 0
while guessloop is not number:
    guess = int(input("Your guess: "))
    if guess == number:
        print("")
        print("You guessed the number!")
        print("The number was:")
        print(number)
        print("")
        print("Please restart the game to start again!")
        guessloop = number
    elif guess < number:
        print("Higher!")
        guessloop = guess
    elif guess > number:
        print("Lower!")
        guessloop = guess
while guessloop == number:
    print("Game has stopped because the user guessed right.")
    guessloop = 0