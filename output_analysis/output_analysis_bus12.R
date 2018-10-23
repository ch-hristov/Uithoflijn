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

# df <- read.csv("output.csv", sep = ';', header = TRUE)
# Store the station names in a variable for easy iteration.
stations <- c("P+R.Uithof", "WKZ", "UMC","Heidelberglaan", "Padualaan","Kromme.Rijn",
               "Galgenwaard", "Vaartsche.Rijn","Centraal.Station")
frequencies <- c(360,180,210,270,300,480,390,510,330,450,540,570,600,420,690,720,630,240,660,780,810,750,840,870,900)
turnaround.times <- c(240,270,300,120,150,180,210)

dir.create(file.path("plots"), showWarnings = FALSE)
dir.create(file.path("plots/bus12"), showWarnings = FALSE)
dir.create(file.path("plots/bus12/CS_TO_PR"), showWarnings = FALSE)
dir.create(file.path("plots/bus12/PR_TO_CS"), showWarnings = FALSE)


bus12.df <- read.csv('output_bus12_validation.csv', stringsAsFactors = FALSE, na.strings = "NaN")
validation.1.df <- read.csv('output_input-data-passengers-01.csv', stringsAsFactors = FALSE, na.strings = "NaN")
validation.2.df <- read.csv('output_input-data-passengers-02.csv', stringsAsFactors = FALSE, na.strings = "NaN")
validation.3.df <- read.csv('output_input-data-passengers-03.csv', stringsAsFactors = FALSE, na.strings = "NaN")
validation.4.df <- read.csv('output_input-data-passengers-04.csv', stringsAsFactors = FALSE, na.strings = "NaN")
validation.6.df <- read.csv('output_input-data-passengers-06.csv', stringsAsFactors = FALSE, na.strings = "NaN")
validation.15.df <- read.csv('output_input-data-passengers-015.csv', stringsAsFactors = FALSE, na.strings = "NaN")
validation.25.df <- read.csv('output_input-data-passengers-025.csv', stringsAsFactors = FALSE, na.strings = "NaN")

#########
# Open multiple files and average, we 'll plot later
########
# Start with the bus data
bus.12 <- list.files(path = "answers_1", pattern = "output_bus12_validation_", full.names = T)

output.data <- data.frame()

for (file in bus.12) {
    data <- read.csv(file, header = T)
    output.data <- rbind(output.data, data)
}

averaged.12 <- aggregate(. ~ tramcnt + q + freq, FUN=mean, data=output.data)


for (file in filenames){
    for (turn in turnaround.times) {
        for (trams in 1:20) {
            for (freq in frequencies) {
                stat.df <- read.csv(paste("stat/STAT_", file, "_", freq, "_", trams, "_", turn, ".txt", sep = "" ), stringsAsFactors = T, na.strings = "NaN")
                stat.df$avg_wait_time <- round(stat.df$avg_wait_time, digits = 2)
                
                stat.df[1,1]
                
                cs_to_pr <- stat.df[c(1:8),]
                
                pr_to_cs <- stat.df[c(9:16),]
                
                png(paste("plots/CS_TO_PR/serviced_passengers_per_station_", file, "_", freq, "_", trams, "_", turn, ".png",sep = ""))
                print(
                    ggplot(cs_to_pr, aes(x=Station, y=total_passengers_serviced)) +
                        geom_bar(stat = "identity", position = 'dodge') +
                        geom_text(aes(label=avg_wait_time), position=position_dodge(width=0.9), vjust=-0.25) +
                        ylab("Serviced passengers") +
                        scale_fill_discrete(name = "Data") +
                        annotate("text", x=Inf, y=Inf, label=paste("Freq = ", freq), vjust=1.1, hjust=1) + 
                        annotate("text", x=Inf, y=Inf, label=paste("Trams = ", trams), vjust=2.3, hjust=1) + 
                        annotate("text", x=Inf, y=Inf, label=paste("Turnaround = ", turn), vjust=3.5, hjust=1) + 
                        theme(axis.text.x = element_text(angle = 45, hjust = 1))
                        
                )
                dev.off()
                
                png(paste("plots/PR_TO_CS/serviced_passengers_per_station_", file, "_", freq, "_", trams, "_", turn, ".png",sep = ""))
                print(
                    ggplot(pr_to_cs, aes(x=Station, y=total_passengers_serviced)) +
                        geom_bar(stat = "identity", position = 'dodge') +
                        geom_text(aes(label=avg_wait_time), position=position_dodge(width=0.9), vjust=-0.25) +
                        ylab("Serviced passengers") +
                        scale_fill_discrete(name = "Data") +
                        annotate("text", x=Inf, y=Inf, label=paste("Freq = ", freq), vjust=1.1, hjust=1) + 
                        annotate("text", x=Inf, y=Inf, label=paste("Trams = ", trams), vjust=2.3, hjust=1) + 
                        annotate("text", x=Inf, y=Inf, label=paste("Turnaround = ", turn), vjust=3.5, hjust=1) + 
                        theme(axis.text.x = element_text(angle = 45, hjust = 1))
                    
                )
                dev.off()
            }
        }
    }
}

dir.create(file.path("plots/bus12/tramcnt"), showWarnings = FALSE)
for (q in turnaround.times){
    png(filename = paste("plots/bus12/tramcnt/bus12_validation_serviced_",q,".png",sep = ""), width = 300, height = 350)
    print(
        ggplot(data = averaged.12[which(averaged.12$q == q),]) +
            geom_line(aes(x=tramcnt, y=serviced, colour=factor(freq))) +
            xlab("Tram count") +
            ylab("Serviced passengers") + 
            annotate("text", x=Inf, y=Inf, label=paste("q = ", q), vjust=1.1, hjust=1) + 
            labs(color = "Turnaround times")
    )
    dev.off()
    
    png(filename = paste("plots/bus12/tramcnt/bus12_validation_punctuality_",q,".png",sep = ""), width = 300, height = 350)
    print(
        ggplot(data = averaged.12[which(averaged.12$q == q),]) +
            geom_line(aes(x=tramcnt, y=punctuality, colour=factor(freq))) +
            xlab("Tram count") +
            ylab("Punctuality (seconds)") + 
            annotate("text", x=Inf, y=Inf, label=paste("q = ", q), vjust=1.1, hjust=1) + 
            labs(color = "Turnaround times")
    )
    dev.off()
    
    png(filename = paste("plots/bus12/tramcnt/bus12_validation_waitingtime_",q,".png",sep = ""), width = 300, height = 350)
    print(
        ggplot(data = averaged.12[which(averaged.12$q == q),]) +
            geom_line(aes(x=tramcnt, y=total_avg_waiting_time, colour=factor(freq))) +
            xlab("Tram count") +
            ylab("Average Waiting time (seconds)") + 
            annotate("text", x=Inf, y=Inf, label=paste("q = ", q), vjust=1.1, hjust=1) + 
            labs(color = "Turnaround times")
    )
    dev.off()
}




dir.create(file.path("plots/bus12/freq"), showWarnings = FALSE)
for (q in turnaround.times){
    png(filename = paste("plots/bus12/freq/bus12_validation_avgwait_", q, ".png",sep = ""), width = 300, height = 350)
    print(
        ggplot(averaged.12[which(averaged.12$q == q),], aes(x = freq, y = total_avg_waiting_time)) +
            geom_bar(stat = "identity", position = 'dodge') +
            xlab("Frequency (seconds)") +
            ylab("Average Waiting time (seconds)") + 
            annotate("text", x=Inf, y=Inf, label=paste("q = ", q), vjust=1.1, hjust=1)
    )
    dev.off()
    
    
    png(filename = paste("plots/bus12/freq/bus12_validation_punctuality_", q, ".png",sep = ""), width = 300, height = 350)
    print(
        ggplot(averaged.12[which(averaged.12$q == q),], aes(x = freq, y = punctuality)) +
            geom_bar(stat = "identity", position = 'dodge') +
            xlab("Frequency (seconds)") +
            ylab("Punctuality (seconds)") + 
            annotate("text", x=Inf, y=Inf, label=paste("q = ", q), vjust=1.1, hjust=1)
    )
    dev.off()
    
    
    png(filename = paste("plots/bus12/freq/bus12_validation_serviced_", q, ".png",sep = ""), width = 300, height = 350)
    print(
        ggplot(averaged.12[which(averaged.12$q == q),], aes(x = freq, y = serviced)) +
            geom_bar(stat = "identity", position = 'dodge') +
            xlab("Frequency (seconds)") +
            ylab("Serviced Passengers") + 
            annotate("text", x=Inf, y=Inf, label=paste("q = ", q), vjust=1.1, hjust=1)
    )
    dev.off()
    
    png(filename = paste("plots/bus12/highlateness/bus12_validation_tramcnt_", q, ".png",sep = ""), width = 300, height = 350)
    print(
        ggplot(averaged.12[which(averaged.12$q == q),]) +
            geom_line(aes(x=tramcnt, y=highLatenessTramsCount, colour=factor(freq))) +
            xlab("Tram count") +
            ylab("High Lateness Tram count") + 
            annotate("text", x=Inf, y=Inf, label=paste("q = ", q), vjust=1.1, hjust=1) + 
            labs(color = "Turnaround times")
    )
    dev.off()
}


dir.create(file.path("plots/bus12/turnaround"), showWarnings = FALSE)
dir.create(file.path("plots/bus12/highlateness"), showWarnings = FALSE)
for (freq in frequencies){
    png(filename = paste("plots/bus12/turnaround/bus12_validation_serviced_", freq, ".png",sep = ""), width = 300, height = 350)
    print(
        ggplot(averaged.12[which(averaged.12$freq == freq),], aes(x = q, y = total_avg_waiting_time)) +
            geom_bar(stat = "identity", position = 'dodge') +
            xlab("Frequency (seconds)") +
            ylab("Average Waiting time (seconds)") + 
            annotate("text", x=Inf, y=Inf, label=paste("Freq = ", freq), vjust=1.1, hjust=1)
    )
    dev.off()
    
    png(filename = paste("plots/bus12/turnaround/bus12_validation_punctuality_", freq, ".png",sep = ""), width = 300, height = 350)
    print(
        ggplot(averaged.12[which(averaged.12$freq == freq),], aes(x = q, y = punctuality)) +
            geom_bar(stat = "identity", position = 'dodge') +
            xlab("Frequency (seconds)") +
            ylab("Punctuality (seconds)") + 
            annotate("text", x=Inf, y=Inf, label=paste("Freq = ", freq), vjust=1.1, hjust=1)
    )
    dev.off()
    
    
    png(filename = paste("plots/bus12/turnaround/bus12_validation_serviced_", freq, ".png",sep = ""), width = 300, height = 350)
    print(
        ggplot(averaged.12[which(averaged.12$freq == freq),], aes(x = q, y = serviced)) +
            geom_bar(stat = "identity", position = 'dodge') +
            xlab("Frequency (seconds)") +
            ylab("Serviced Passengers") + 
            annotate("text", x=Inf, y=Inf, label=paste("Freq = ", freq), vjust=1.1, hjust=1)
    )
    dev.off()
    
    # png(filename = paste("plots/bus12/highlateness/bus12_validation_tramcnt_", freq, ".png",sep = ""), width = 300, height = 350)
    # print(
    #     ggplot(averaged.12[which(averaged.12$freq == freq),]) +
    #         geom_line(aes(x=tramcnt, y=highLatenessTramsCount, colour=factor(q))) +
    #         xlab("Tram count") +
    #         ylab("High Lateness Tram count") + 
    #         annotate("text", x=Inf, y=Inf, label=paste("Freq = ", freq), vjust=1.1, hjust=1) + 
    #         labs(color = "Turnaround times")
    # )
    # dev.off()
}

png(filename = paste("plots/bus12/highlateness/bus12_validation_tramcnt_", freq, ".png",sep = ""), width = 300, height = 350)
print(
    ggplot(averaged.12[which(averaged.12$q == q),]) +
        geom_line(aes(x=tramcnt, y=highLatenessTramsCount, colour=factor(freq))) +
        xlab("Tram count") +
        ylab("High Lateness Tram count") + 
        annotate("text", x=Inf, y=Inf, label=paste("q = ", q), vjust=1.1, hjust=1) + 
        labs(color = "Turnaround times")
)
dev.off()





