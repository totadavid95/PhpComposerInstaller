using System;
using System.Collections.Generic;

namespace PhpComposerInstaller {
    /// <summary>
    /// Represents a command-line option with a boolean value and a description.
    /// </summary>
    /// TODO: add support for other types of options, if needed.
    internal class Option {
        /// <summary>
        /// Gets or sets the boolean value of the option.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Gets the description of the option.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class.
        /// </summary>
        /// <param name="value">The initial value of the option.</param>
        /// <param name="description">The description of the option.</param>
        public Option(bool value, string description) {
            Value = value;
            Description = description;
        }
    }

    /// <summary>
    /// Handles command-line options and provides methods for processing arguments.
    /// </summary>
    internal class OptionHandler {
        private readonly Dictionary<string, Option> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionHandler"/> class.
        /// </summary>
        /// <param name="options">The dictionary of command-line options.</param>
        public OptionHandler(Dictionary<string, Option> options) {
            this.options = options;
        }

        /// <summary>
        /// Handles command-line arguments and updates the state of options accordingly.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public void HandleArgs(string[] args) {
            for (int i = 0; i < args.Length; i++) {
                var arg = args[i].ToLower();

                // Special case: --help
                if (arg == "--help") {
                    PrintHelp();
                    Environment.Exit(0);
                }

                // Option arguments must start with "--" and they can be negated with "--no-" prefix
                if (!arg.StartsWith("--")) {
                    Console.WriteLine("Unknown argument: " + args[i]);
                    continue;
                }

                arg = arg.Substring("--".Length);
                var state = !arg.StartsWith("no-");

                if (!state) {
                    arg = arg.Substring("no-".Length);
                }

                if (!options.ContainsKey(arg)) {
                    // Just a warning, we don't want to exit the program
                    Console.WriteLine("Unknown argument: " + args[i]);
                    continue;
                }

                // Use the handler function to set the value in the options dictionary
                HandleOption(arg, state);
            }
        }

        /// <summary>
        /// Handles a specific command-line option and updates its value.
        /// </summary>
        /// <param name="optionName">The name of the option to handle.</param>
        /// <param name="state">The new state of the option.</param>
        private void HandleOption(string optionName, bool state) {
            if (options.TryGetValue(optionName, out var option)) {
                option.Value = state;
            }
        }

        /// <summary>
        /// Prints the status of an option (ON or OFF).
        /// </summary>
        /// <param name="option">The option to print the status for.</param>
        /// <returns>The status of the option as a string.</returns>
        private string PrintOptionStatus(Option option) {
            return option.Value ? "ON" : "OFF";
        }

        /// <summary>
        /// Checks if a specific option is enabled.
        /// </summary>
        /// <param name="optionName">The name of the option to check.</param>
        /// <returns>True if the option is enabled; otherwise, false.</returns>
        public bool IsOptionEnabled(string optionName) {
            if (options.TryGetValue(optionName, out var option)) {
                return option.Value;
            }

            return false;
        }

        /// <summary>
        /// Prints the help message.
        /// </summary>
        private void PrintHelp() {
            const string tool = "PhpComposerInstaller.exe";
            const string readmeUrl = "https://github.com/totadavid95/PhpComposerInstaller/blob/master/README.md";

            Console.WriteLine($"Usage: {tool} [options]");
            Console.WriteLine(" ");
            Console.WriteLine("Options:");

            foreach (var option in options) {
                Console.WriteLine($"  --{option.Key,-12} {option.Value.Description} (default: {PrintOptionStatus(option.Value)})");
            }

            Console.WriteLine($"  --{"help",-12} Prints this help message");
            Console.WriteLine(" ");
            Console.WriteLine("Examples:");
            Console.WriteLine($"  '{tool} --no-composer' is installs PHP but not Composer");
            Console.WriteLine($"  '{tool} --uninstall' uninstalls PHP and Composer, if they are installed");
            Console.WriteLine(" ");
            Console.WriteLine($"For more information, please visit {readmeUrl}");
        }
    }
}
