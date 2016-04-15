# GeneticAlgorithmSelfDriving
Little car bots that use a genetic algorithm to solve the best way around a course. Uses Unity
Cars are given fitness based on how far down the track they went and how safely they did so. Each car gets a JSON object representing their "chromosome"
We then save json data for each generation ran, breed the best cars together to form children, add mutation to children for randomness,
eliminate the bottom half of the older generation, then test the children and repeat the process until a child can complete the course.
Options to run only the new children per generation or show parents as well are both available in the inspector.
Color coding represents the fitness of all cars.
Input commands are based on fully human usable controls created for the cars.
100% adaptable to any course challenge (as long as there is a possible solution)
