###################################################################################################
# output_analysis_plots.R                                                                          #
# @author:  Ioannis Pavlos Panteliadis <i.p.panteliadis@students.uu.nl>                           #
# @brief:   This file contains function declarations and implementations for the input analysis   #
#           performed for the project in Algorithms and Decision Support of Utrecht University    #
###################################################################################################

require (reshape)
require (ggplot2)

# df <- read.csv("output.csv", sep = ';', header = TRUE)
# Store the station names in a variable for easy iteration.
stations <- c("P+R.Uithof", "WKZ", "UMC","Heidelberglaan", "Padualaan","Kromme.Rijn",
               "Galgenwaard", "Vaartsche.Rijn","Centraal.Station")


# Read the data
# PR to CS
df.PR.to.CS <- read.csv("passenger_waiting_times_at_stations_PR_to_CS.csv", header = TRUE) 

for (st in stations){
    df <- df.PR.to.CS[which(df.PR.to.CS$station==st),]
    
    plot <- ggplot(df, aes(x = time, y = average_waiting_time)) +
        geom_bar(stat = "identity") + 
        ylab("Waiting time (sec)") +
        scale_x_discrete(limits=c("6-7","7-8","8-9","9-10","10-11","11-12","12-13","13-14","14-15","15-16","16-17","17-18","18-19","19-20","20-21","21-22"))
    
    png(filename = paste("plots/",st,"_waiting_times.png",sep=""))
    print(plot)
    dev.off()
}


data.from.output <- read.csv('output.csv', header = T, sep = ';')

data.from.output.trimmed <- data.from.output[(data.from.output$punctuality < max(data.from.output$punctuality) - 900000) ,] 

data.from.output.trimmed$freq <- as.factor(data.from.output.trimmed$freq)
data.from.output.trimmed$tramcnt <- as.factor(data.from.output.trimmed$tramcnt)
data.from.output.trimmed$q <- as.factor(data.from.output.trimmed$q)

punctuality.plot <- ggplot(data.from.output.trimmed, aes(x = freq, y = punctuality, fill = q)) +
    geom_bar(stat = "identity", position = position_dodge()) + 
    geom_hline(yintercept = 1020) + 
    geom_text(aes(label=tramcnt), position=position_dodge(width=0.9), vjust=-0.25) + 
    scale_fill_discrete(name = "Turnaround time (sec)") + 
    xlab("Frequency (sec)") + 
    ylab("Time for roundtrip (sec)")
    #scale_y_continuous(breaks = sort(c(seq(min(punctuality$punctuality), max(punctuality$punctuality), length.out = 5), 1020)))

png(file="plots/punctuality.plot.png")
punctuality.plot
dev.off()






