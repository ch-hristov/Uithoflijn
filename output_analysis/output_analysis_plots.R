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


# # Read the data
# # PR to CS
# df.PR.to.CS <- read.csv("passenger_waiting_times_at_stations_PR_to_CS.csv", header = TRUE) 
# 
# for (st in stations){
#     df <- df.PR.to.CS[which(df.PR.to.CS$station==st),]
#     
#     plot <- ggplot(df, aes(x = time, y = average_waiting_time)) +
#         geom_bar(stat = "identity") + 
#         ylab("Waiting time (sec)") +
#         scale_x_discrete(limits=c("6-7","7-8","8-9","9-10","10-11","11-12","12-13","13-14","14-15","15-16","16-17","17-18","18-19","19-20","20-21","21-22"))
#     
#     png(filename = paste("plots/",st,"_waiting_times.png",sep=""))
#     print(plot)
#     dev.off()
# }
# 
# 
# data.from.output <- read.csv('output.csv', header = T, sep = ';')
# 
# data.from.output.trimmed <- data.from.output[(data.from.output$punctuality < max(data.from.output$punctuality) - 900000) ,] 
# 
# data.from.output.trimmed$freq <- as.factor(data.from.output.trimmed$freq)
# data.from.output.trimmed$tramcnt <- as.factor(data.from.output.trimmed$tramcnt)
# data.from.output.trimmed$q <- as.factor(data.from.output.trimmed$q)
# 
# punctuality.plot <- ggplot(data.from.output.trimmed, aes(x = freq, y = punctuality, fill = q)) +
#     geom_bar(stat = "identity", position = position_dodge()) + 
#     geom_hline(yintercept = 1020) + 
#     geom_text(aes(label=tramcnt), position=position_dodge(width=0.9), vjust=-0.25) + 
#     scale_fill_discrete(name = "Turnaround time (sec)") + 
#     xlab("Frequency (sec)") + 
#     ylab("Time for roundtrip (sec)")
#     #scale_y_continuous(breaks = sort(c(seq(min(punctuality$punctuality), max(punctuality$punctuality), length.out = 5), 1020)))
# 
# png(file="plots/punctuality.plot.png")
# punctuality.plot
# dev.off()


frequencies <- c(180, 360, 540, 720, 900)

bus12.df <- read.csv('output_bus12_validation.csv', stringsAsFactors = FALSE)
validation.1.df <- read.csv('output_input-data-passengers-01.csv', stringsAsFactors = FALSE)
validation.2.df <- read.csv('output_input-data-passengers-02.csv', stringsAsFactors = FALSE)
validation.3.df <- read.csv('output_input-data-passengers-03.csv', stringsAsFactors = FALSE)
validation.4.df <- read.csv('output_input-data-passengers-04.csv', stringsAsFactors = FALSE)
validation.6.df <- read.csv('output_input-data-passengers-06.csv', stringsAsFactors = FALSE)
validation.15.df <- read.csv('output_input-data-passengers-015.csv', stringsAsFactors = FALSE)
validation.25.df <- read.csv('output_input-data-passengers-025.csv', stringsAsFactors = FALSE)

# Compine the dfs in to 1.
df.melted.1 <- rbind(bus12.df, validation.1.df, validation.2.df, validation.3.df)
df.melted.2 <- rbind(validation.3.df, validation.4.df, validation.6.df, validation.15.df, validation.25.df)


## Barplots for the serviced passengers
for (freq in frequencies){
    png(paste("plots/serviced_passengers1_freq", freq, ".png",sep = ""))
    print(
        ggplot(df.melted.1[which(df.melted.1$freq == freq),], aes(x=factor(tramcnt), y=serviced, fill=file)) +
            geom_bar(stat = "identity", position = 'dodge') +
            xlab("Trams") +
            ylab("Serviced passengers") +
            scale_fill_discrete(name = "Data") +
            annotate("text", x=Inf, y = Inf, label=paste("Freq = ", freq), vjust=1.1, hjust=1) 
    )
    dev.off()
    
    png(paste("plots/serviced_passengers2_freq", freq, ".png",sep = ""))
    print(
        ggplot(df.melted.2[which(df.melted.2$freq == freq),], aes(x=factor(tramcnt), y=serviced, fill=file)) +
            geom_bar(stat = "identity", position = 'dodge') +
            xlab("Trams") +
            ylab("Serviced passengers") +
            scale_fill_discrete(name = "Data") +
            annotate("text", x=Inf, y = Inf, label=paste("Freq = ", freq), vjust=1.1, hjust=1)
    )
    dev.off()
}


## Waiting times
for (freq in frequencies) {
    png(paste("plots/waiting_times1_freq", freq, ".png",sep = ""))
    print(
        ggplot(df.melted.1[which(df.melted.1$freq == freq),], aes(x=tramcnt, y=total_waiting_time, colour=file)) +
            geom_line() +
            scale_x_discrete(limits=c(1:20)) +
            xlab("Trams") +
            ylab("Waiting Time (sec)") +
            labs(color = "Data") +
            annotate("text", x=Inf, y = Inf, label=paste("Freq = ", freq), vjust=1.1, hjust=1) 
    )
    dev.off()
    
    png(paste("plots/waiting_times2_freq", freq, ".png",sep = ""))
    print(
        ggplot(df.melted.2[which(df.melted.2$freq == freq),], aes(x=tramcnt, y=total_waiting_time, colour=file)) +
            geom_line() +
            scale_x_discrete(limits=c(1:20)) +
            xlab("Trams") +
            ylab("Waiting Time (sec)") +
            labs(color = "Data") +
            annotate("text", x=Inf, y = Inf, label=paste("Freq = ", freq), vjust=1.1, hjust=1)
    )
    dev.off()
}



