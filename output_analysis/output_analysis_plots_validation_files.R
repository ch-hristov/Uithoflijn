###################################################################################################
# output_analysis_plots.R                                                                          #
# @author:  Ioannis Pavlos Panteliadis <i.p.panteliadis@students.uu.nl>                           #
# @brief:   This file contains function declarations and implementations for the input analysis   #
#           performed for the project in Algorithms and Decision Support of Utrecht University    #
###################################################################################################

if (!require("reshape")) {
    library("reshape")
    install.packages("reshape")
}

if (!require("ggplot2")) {
    library("ggplot2")
    install.packages("ggplot2")
}

if (!require("data.table")) {
    install.packages("data.table", dependencies=TRUE)
    library("data.table")
}

## Create directories for storing.
dir.create(file.path("plots/validation"), showWarnings = FALSE)
dir.create(file.path("plots/validation/highLateness"), showWarnings = FALSE)
dir.create(file.path("plots/validation/avg_wait"), showWarnings = FALSE)



#########
# Open multiple files and average, we 'll plot later
########
# Start with the first validation file
filenames.2 <- list.files(path = "answers", pattern = "output_input-data-passengers-02_", full.names = T)

output.data <- data.frame()

for (file in filenames.2) {
    data <- read.csv(file, header = T)
    output.data <- rbind(output.data, data)
}

averaged.2 <- aggregate(. ~ tramcnt, FUN=mean, data=output.data)
averaged.2 <- averaged.2[which(averaged.2$q == 300 & averaged.2$freq == 240),]

# output_input-data-passengers-03_
filenames.3 <- list.files(path = "answers", pattern = "output_input-data-passengers-03_", full.names = T)

output.data <- data.frame()

for (file in filenames.3) {
    data <- read.csv(file, header = T)
    output.data <- rbind(output.data, data)
}

averaged.3 <- aggregate(. ~ tramcnt, FUN=mean, data=output.data)
averaged.3 <- averaged.3[which(averaged.3$q == 300 & averaged.3$freq == 240),]

# output_input-data-passengers-04_
filenames.4 <- list.files(path = "answers", pattern = "output_input-data-passengers-04_", full.names = T)

output.data <- data.frame()

for (file in filenames.4) {
    data <- read.csv(file, header = T)
    output.data <- rbind(output.data, data)
}

averaged.4 <- aggregate(. ~ tramcnt, FUN=mean, data=output.data)
averaged.4 <- averaged.4[which(averaged.4$q == 300 & averaged.4$freq == 240),]

# output_input-data-passengers-06_
filenames.6 <- list.files(path = "answers", pattern = "output_input-data-passengers-06_", full.names = T)

output.data <- data.frame()

for (file in filenames.6) {
    data <- read.csv(file, header = T)
    output.data <- rbind(output.data, data)
}

averaged.6 <- aggregate(. ~ tramcnt, FUN=mean, data=output.data)
averaged.6 <- averaged.6[which(averaged.6$q == 300 & averaged.6$freq == 240),]



# output_input-data-passengers-025_
filenames.025 <- list.files(path = "answers", pattern = "input-data-passengers-025_", full.names = T)

output.data <- data.frame()

for (file in filenames.025) {
    data <- read.csv(file, header = T)
    output.data <- rbind(output.data, data)
}

averaged.025 <- aggregate(. ~ tramcnt, FUN=mean, data=output.data)
averaged.025 <- averaged.025[which(averaged.025$q == 300 & averaged.025$freq == 240),]

######################
# Plot the graphs
######################

# File output_input-data-passengers-02.csv
png(filename = "plots/validation/avg_wait/input-data-passengers-02_avg_wait.png", width = 300, height = 350)
ggplot(averaged.2, aes(x=factor(tramcnt), y=total_avg_waiting_time)) +
    geom_bar(stat = "identity", position = 'dodge') +
    xlab("Issued trams") +
    ylab("Average wait (seconds)") +
    annotate("text", x=Inf, y=Inf, label=paste("input-data-passengers-02.csv"), vjust=1.1, hjust=1)
dev.off()

png(filename = "plots/validation/highLateness/input-data-passengers-02_late_trams.png", width = 300, height = 350)
ggplot(averaged.2, aes(x=factor(tramcnt), y=highLatenessTramsCount)) +
    geom_bar(stat = "identity", position = 'dodge') +
    xlab("Issued trams") +
    ylab("Number of Trams") +
    annotate("text", x=Inf, y=Inf, label=paste("input-data-passengers-02.csv"), vjust=1.1, hjust=1)
dev.off()

########

# File output_input-data-passengers-03.csv
png(filename = "plots/validation/avg_wait/input-data-passengers-03_avg_wait.png", width = 300, height = 350)
ggplot(averaged.3, aes(x=factor(tramcnt), y=total_avg_waiting_time)) +
    geom_bar(stat = "identity", position = 'dodge') +
    xlab("Issued trams") +
    ylab("Average wait (seconds)") +
    annotate("text", x=Inf, y=Inf, label=paste("input-data-passengers-03.csv"), vjust=1.1, hjust=1)
dev.off()

png(filename = "plots/validation/highLateness/input-data-passengers-03_late_trams.png", width = 300, height = 350)
ggplot(averaged.3, aes(x=factor(tramcnt), y=highLatenessTramsCount)) +
    geom_bar(stat = "identity", position = 'dodge') +
    xlab("Issued trams") +
    ylab("Number of Trams") +
    annotate("text", x=Inf, y=Inf, label=paste("input-data-passengers-03.csv"), vjust=1.1, hjust=1)
dev.off()


########

# File output_input-data-passengers-04.csv
png(filename = "plots/validation/avg_wait/input-data-passengers-04_avg_wait.png", width = 300, height = 350)
ggplot(averaged.4, aes(x=factor(tramcnt), y=total_avg_waiting_time)) +
    geom_bar(stat = "identity", position = 'dodge') +
    xlab("Issued trams") +
    ylab("Average wait (seconds)") +
    annotate("text", x=Inf, y=Inf, label=paste("input-data-passengers-04.csv"), vjust=1.1, hjust=1)
dev.off()

png(filename = "plots/validation/highLateness/input-data-passengers-04_late_trams.png", width = 300, height = 350)
ggplot(averaged.4, aes(x=factor(tramcnt), y=highLatenessTramsCount)) +
    geom_bar(stat = "identity", position = 'dodge') +
    xlab("Issued trams") +
    ylab("Number of Trams") +
    annotate("text", x=Inf, y=Inf, label=paste("input-data-passengers-04.csv"), vjust=1.1, hjust=1)
dev.off()

########

# File output_input-data-passengers-06.csv
png(filename = "plots/validation/avg_wait/input-data-passengers-06_avg_wait.png", width = 300, height = 350)
ggplot(averaged.6, aes(x=factor(tramcnt), y=total_avg_waiting_time)) +
    geom_bar(stat = "identity", position = 'dodge') +
    xlab("Issued trams") +
    ylab("Average wait (seconds)") +
    annotate("text", x=Inf, y=Inf, label=paste("input-data-passengers-06.csv"), vjust=1.1, hjust=1)
dev.off()

png(filename = "plots/validation/highLateness/input-data-passengers-06_late_trams.png", width = 300, height = 350)
ggplot(averaged.6, aes(x=factor(tramcnt), y=highLatenessTramsCount)) +
    geom_bar(stat = "identity", position = 'dodge') +
    xlab("Issued trams") +
    ylab("Number of Trams") +
    annotate("text", x=Inf, y=Inf, label=paste("input-data-passengers-06.csv"), vjust=1.1, hjust=1)
dev.off()

########
# File output_input-data-passengers-025.csv
png(filename = "plots/validation/avg_wait/input-data-passengers-025_avg_wait.png", width = 300, height = 350)
ggplot(averaged.025, aes(x=factor(tramcnt), y=total_avg_waiting_time)) +
    geom_bar(stat = "identity", position = 'dodge') +
    xlab("Issued trams") +
    ylab("Average wait (seconds)") +
    annotate("text", x=Inf, y=Inf, label=paste("input-data-passengers-025.csv"), vjust=1.1, hjust=1)
dev.off()

png(filename = "plots/validation/highLateness/input-data-passengers-025_late_trams.png", width = 300, height = 350)
ggplot(averaged.025, aes(x=factor(tramcnt), y=highLatenessTramsCount)) +
    geom_bar(stat = "identity", position = 'dodge') +
    xlab("Issued trams") +
    ylab("Number of Trams") +
    annotate("text", x=Inf, y=Inf, label=paste("input-data-passengers-025.csv"), vjust=1.1, hjust=1)
dev.off()

