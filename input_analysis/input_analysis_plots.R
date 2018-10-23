###################################################################################################
# input_analysis_plots.R                                                                          #
# @author:  Ioannis Pavlos Panteliadis <i.p.panteliadis@students.uu.nl>                           #
# @brief:   This file contains function declarations and implementations for the input analysis   #
#           performed for the project in Algorithms and Decision Support of Utrecht University    #
###################################################################################################

require (reshape)
require (ggplot2)

# Ration files
# first line is 24Hr, 2nd first peak 7-9, 3rd second peak 16-18 
pr.to.cs <- read.csv("pr.to.cs.exiting.ratios.csv", header = TRUE)
pr.to.cs2 <- as.data.frame(t(pr.to.cs))
colnames(pr.to.cs2)[1] <- 'Normal'
colnames(pr.to.cs2)[2] <- 'Morning.peak'
colnames(pr.to.cs2)[3] <- 'Afternoon.peak'
pr.to.cs2$Stations <- as.factor(1:9)

GG1 <- ggplot() +
    geom_line(data = pr.to.cs2, aes(x = Stations, y = Normal, group = 1, colour = colnames(pr.to.cs2)[1])) +
    geom_line(data = pr.to.cs2, aes(x = Stations, y = Morning.peak, group = 2, colour = colnames(pr.to.cs2)[2])) +
    geom_line(data = pr.to.cs2, aes(x = Stations, y = Afternoon.peak, group = 3, colour = colnames(pr.to.cs2)[3])) +
    scale_x_discrete(breaks=c("1":"9"),
                     labels=c("P+R.Uithof","WKZ","UMC" ,"Heidelberglaan",   "Padualaan","Kromme.Rijn",
                              "Galgenwaard", "Vaartsche.Rijn","Centraal.Station")) +
    theme(axis.text.x = element_text(angle=45)) +
    ylab("Disembarkation / Occupancy ration") +
    xlab("") +
    theme(legend.title=element_blank())

png("pr_to_cs_exiting_ratios.png")
GG1
dev.off()


cs.to.pr <- read.csv("cs.to.pr.exiting.ratios.csv", header = TRUE)
cs.to.pr.2 <- as.data.frame(t(cs.to.pr))
colnames(cs.to.pr.2)[1] <- 'Normal'
colnames(cs.to.pr.2)[2] <- 'Morning.peak'
colnames(cs.to.pr.2)[3] <- 'Afternoon.peak'
cs.to.pr.2$Stations <- as.factor(1:9)

GG2 <- ggplot() +
    geom_line(data = cs.to.pr.2, aes(x = Stations, y = Normal, group = 1, colour = colnames(cs.to.pr.2)[1])) +
    geom_line(data = cs.to.pr.2, aes(x = Stations, y = Morning.peak, group = 2, colour = colnames(cs.to.pr.2)[2])) +
    geom_line(data = cs.to.pr.2, aes(x = Stations, y = Afternoon.peak, group = 3, colour = colnames(cs.to.pr.2)[3])) +
    scale_x_discrete(breaks=c("1":"9"),
                     labels=rev(c("P+R.Uithof","WKZ","UMC" ,"Heidelberglaan",   "Padualaan","Kromme.Rijn",
                              "Galgenwaard", "Vaartsche.Rijn","Centraal.Station"))) +
    theme(axis.text.x = element_text(angle=45)) +
    ylab("Disembarkation / Occupancy ration") +
    xlab("") +
    theme(legend.title=element_blank())

png("cs_to_pr_exiting_ratios.png")
GG2
dev.off()
