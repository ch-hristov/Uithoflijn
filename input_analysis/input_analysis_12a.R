###################################################################################################
# input_analysis.R                                                                                #
# @author:  Ioannis Pavlos Panteliadis <i.p.panteliadis@students.uu.nl>                           #
# @brief:   This file contains function declarations and implementations for the input analysis   #
#           performed for the project in Algorithms and Decision Support of Utrecht University    #
###################################################################################################
if (!require("chron")) {
    install.packages("chron")
    library(chron)
}

if (!require("tibble")){
    install.packages(("tibble"))
    library(tibble) 
}

if (!require("reshape")) {
    library("reshape")
    install.packages("reshape")
}

# Plotting libraries
library(dplyr)
library(tidyr)

file.name <- '12a'

# Load the file
all.content = readLines(paste(file.name,".csv", sep = ''))
skip.first = all.content[-1]
passenger.info.route.A = read.csv(textConnection(skip.first), header = TRUE, stringsAsFactors = FALSE)

passenger.info.route.A$Stadion.Galgenwaard <- ceiling((passenger.info.route.A$Stadion.Galgenwaard + passenger.info.route.A$Rubenslaan) / 2)
passenger.info.route.A$Rubenslaan <- NULL

passenger.info.route.A$Stadion.Galgenwaard.1 <- ceiling((passenger.info.route.A$Stadion.Galgenwaard.1 + passenger.info.route.A$Rubenslaan.1) / 2)
passenger.info.route.A$Rubenslaan.1 <- NULL

passenger.info.route.A$Bleekstraat <- ceiling((passenger.info.route.A$Bleekstraat + passenger.info.route.A$Sterrenwijk) / 2)
passenger.info.route.A$Sterrenwijk <- NULL

passenger.info.route.A$Bleekstraat.1 <- ceiling((passenger.info.route.A$Bleekstraat.1 + passenger.info.route.A$Sterrenwijk.1) / 2)
passenger.info.route.A$Sterrenwijk.1 <- NULL


# Create the UMC column for the entering passengers
passenger.info.route.A <- add_column(passenger.info.route.A, "UMC", .before = "AZU")
colnames(passenger.info.route.A)[4] <- "UMC"
passenger.info.route.A <- add_column(passenger.info.route.A, "WKZ", .before = "UMC")
colnames(passenger.info.route.A)[4] <- "WKZ"
passenger.info.route.A <- add_column(passenger.info.route.A, "P+R Uithof", .before = "WKZ")
colnames(passenger.info.route.A)[4] <- "P+R Uithof"

# Create the UMC column for the exiting passengers
passenger.info.route.A <- add_column(passenger.info.route.A, "UMC.1", .before = "AZU.1")
colnames(passenger.info.route.A)[14] <- "UMC.1"
passenger.info.route.A <- add_column(passenger.info.route.A, "WKZ.1", .before = "UMC.1")
colnames(passenger.info.route.A)[14] <- "WKZ.1"
passenger.info.route.A <- add_column(passenger.info.route.A, "P+R Uithof.1", .before = "WKZ.1")
colnames(passenger.info.route.A)[14] <- "P+R Uithof.1"

# Adjust the entering passengers
passenger.info.route.A$UMC <- ceiling(passenger.info.route.A$AZU * (2660/3690))
passenger.info.route.A$WKZ <- ceiling(passenger.info.route.A$AZU * (1015/3690))
passenger.info.route.A$`P+R Uithof` <- ceiling(passenger.info.route.A$AZU * (15/3690))

# Adjust the exiting passengers
passenger.info.route.A$UMC.1 <- ceiling(passenger.info.route.A$AZU.1 * 0)
passenger.info.route.A$WKZ.1 <- ceiling(passenger.info.route.A$AZU.1 * 0)
passenger.info.route.A$`P+R Uithof.1` <- ceiling(passenger.info.route.A$AZU.1 * 0)


passenger.info.route.A$AZU <- NULL
passenger.info.route.A$AZU.1 <- NULL

# write.csv(passenger.info.route.A, file = paste(file.name, ".adjusted.csv", sep = ""), quote = FALSE)

# Now we need to get the averages per trip
averages.per.trip <- format(aggregate(passenger.info.route.A[-c(1, 2, 3)], list(passenger.info.route.A$Departure), mean), digits = 2)
averages.per.trip$Group.1 <- times(paste(averages.per.trip$Group.1, "00", sep = ":"))

exiting.passengers <- averages.per.trip[, c(1, 11:19)]

# pr.to.cs <- data.frame(station = rev(c("Utrecht Centraal", "Vaartsche Rijn", "Galgenwaard", "Kromme Rijn","Padualaan","Heidelberglaan",  "UMC", "WKZ", "P+R Uithof")),
#                        general = c(0, 0.00143747, 0.000338226, 0.00004337218758, 0.018402118, 0.001572231, 0.013160988, 0.026607992, 1),
#                        morning.peak = c(0, 0, 0.000642275, 0.00005481189203, 0.19169781, 0.001543533, 0.039547378, 0.084536474, 1),
#                        afternoon.peak = c(0, 0.00305499, 0.000179395, 0.0000094822, 0.000681801, 0.000750849, 0.009020428, 0.030002259, 1))
#write.csv(pr.to.cs, file = "prognosis.pr.to.cs.csv", quote = FALSE)


# cs.to.pr <- data.frame(station = c("Utrecht Centraal", "Vaartsche Rijn", "Galgenwaard", "Kromme Rijn","Padualaan", "Heidelberglaan",  "UMC", "WKZ", "P+R Uithof"),
#                        general = c(0, 0.086763924, 0.062557774, 0.052848369, 0.517963103, 0.642401152, 0.797902237, 0.977346689, 1),
#                        morning.peak = c(0, 0.036248402, 0.034717693, 0.026773621, 0.538356319, 0.648449401, 0.800210006, 0.974718246, 1),
#                        afternoon.peak = c(0, 0.138608172, 0.10572386, 0.149689112, 0.430903442, 0.522978061, 0.694063584, 0.997447291, 1))
#write.csv(cs.to.pr, file = "prognosis.cs.to.pr.csv", quote = FALSE)


# ENTERING PASSENGERS P+R -> CS
df2 <- averages.per.trip[, c(1:10)]
df2$Group.1[125:nrow(df2)] <- paste("0",df2$Group.1[125:nrow(df2)], sep = "")
df2$Group.1 <- paste("20180930 ", gsub(":", "", df2$Group.1), "00", sep = "")
df2$Time <- as.POSIXct(df2$Group.1, format = "%Y%m%d %H%M%S")
df2$by15 = cut(df2$Time, breaks=c("60 min"))
df2$`P+R Uithof` <- as.numeric(df2$`P+R Uithof`)
df2$WKZ <- as.numeric(df2$WKZ)
df2$UMC <- as.numeric(df2$UMC)
df2$Heidelberglaan <- as.numeric(df2$Heidelberglaan)
df2$Padualaan <- as.numeric(df2$Padualaan)
df2$De.Kromme.Rijn <- as.numeric(df2$De.Kromme.Rijn)
df2$Stadion.Galgenwaard <- as.numeric(df2$Stadion.Galgenwaard)
df2$Bleekstraat <- as.numeric(df2$Bleekstraat)
df2$CS.Centrumzijde <- as.numeric(df2$CS.Centrumzijde)



entering.summary <- c()
entering.summary$`P+R Uithof` <- aggregate(`P+R Uithof` ~ by15, FUN=sum, data=df2)
entering.summary$WKZ <- aggregate(WKZ ~ by15, FUN=sum, data=df2)
entering.summary$UMC <- aggregate(UMC ~ by15, FUN=sum, data=df2)
entering.summary$Heidelberglaan <- aggregate(Heidelberglaan ~ by15, FUN=sum, data=df2)
entering.summary$Padualaan <- aggregate(Padualaan ~ by15, FUN=sum, data=df2)
entering.summary$De.Kromme.Rijn <- aggregate(De.Kromme.Rijn ~ by15, FUN=sum, data=df2)
entering.summary$Stadion.Galgenwaard <- aggregate(Stadion.Galgenwaard ~ by15, FUN=sum, data=df2)
entering.summary$Bleekstraat <- aggregate(Bleekstraat ~ by15, FUN=sum, data=df2)
entering.summary$CS.Centrumzijde <- aggregate(CS.Centrumzijde ~ by15, FUN=sum, data=df2)



# Do the same for the exiting passengers P+R -> CS
df3 <- exiting.passengers
df3$Group.1[125:nrow(df3)] <- paste("0",df3$Group.1[125:nrow(df3)], sep = "")
df3$Group.1 <- paste("20180930 ", gsub(":", "", df3$Group.1), "00", sep = "")
df3$Time <- as.POSIXct(df3$Group.1, format = "%Y%m%d %H%M%S")
df3$by15 = cut(df3$Time, breaks=c("60 min"))
df3$`P+R Uithof.1` <- as.numeric(df3$`P+R Uithof.1`)
df3$WKZ.1 <- as.numeric(df3$WKZ.1)
df3$UMC.1 <- as.numeric(df3$UMC.1)
df3$Heidelberglaan.1 <- as.numeric(df3$Heidelberglaan.1)
df3$Padualaan.1 <- as.numeric(df3$Padualaan.1)
df3$De.Kromme.Rijn.1 <- as.numeric(df3$De.Kromme.Rijn.1)
df3$Stadion.Galgenwaard.1 <- as.numeric(df3$Stadion.Galgenwaard.1)
df3$Bleekstraat.1 <- as.numeric(df3$Bleekstraat.1)
df3$CS.Centrumzijde.1 <- as.numeric(df3$CS.Centrumzijde.1)


exiting.summary <- c()
exiting.summary$`P+R Uithof.1` <- aggregate(`P+R Uithof.1` ~ by15, FUN=sum, data=df3)
exiting.summary$WKZ.1 <- aggregate(WKZ.1 ~ by15, FUN=sum, data=df3)
exiting.summary$UMC.1 <- aggregate(UMC.1 ~ by15, FUN=sum, data=df3)
exiting.summary$Heidelberglaan.1 <- aggregate(Heidelberglaan.1 ~ by15, FUN=sum, data=df3)
exiting.summary$Padualaan.1 <- aggregate(Padualaan.1 ~ by15, FUN=sum, data=df3)
exiting.summary$De.Kromme.Rijn.1 <- aggregate(De.Kromme.Rijn.1 ~ by15, FUN=sum, data=df3)
exiting.summary$Stadion.Galgenwaard.1 <- aggregate(Stadion.Galgenwaard.1 ~ by15, FUN=sum, data=df3)
exiting.summary$Bleekstraat.1 <- aggregate(Bleekstraat.1 ~ by15, FUN=sum, data=df3)
exiting.summary$CS.Centrumzijde.1 <- aggregate(CS.Centrumzijde.1 ~ by15, FUN=sum, data=df3)




stations <- c("P+R Uithof", "WKZ", "UMC","Heidelberglaan", "Padualaan","Kromme Rijn",
              "Galgenwaard", "Vaartscherijn","Centrumzijde Centraal Station")

entering.summary <- as.data.frame(entering.summary)
entering.summary[, c(3,5,7,9,11,13,15,17)] <- NULL
colnames(entering.summary) <- c("by60", stations)
write.csv(entering.summary, file = paste(file.name, ".entering.60.intervals.csv", sep = ""), quote = FALSE)

exiting.summary <- as.data.frame(exiting.summary)
exiting.summary[, c(3,5,7,9,11,13,15,17)] <- NULL
colnames(exiting.summary) <- c("by60", stations)


# Generate the file that will look like the validation files
# First do the entering passengers
entering.melted <- melt(entering.summary, id.vars =c("by60"))
colnames(entering.melted) <- c("Time", "Station", "PassIn")
entering.melted$Station <- as.factor(entering.melted$Station)
entering.melted$Time <- as.factor(entering.melted$Time)

entering.melted <- entering.melted[order(entering.melted$Time, entering.melted$Station),]

# Then the exiting
exiting.melted <- melt(exiting.summary, id.vars =c("by60"))
colnames(exiting.melted) <- c("Time", "Station", "PassOut")
exiting.melted$Station <- as.factor(exiting.melted$Station)
exiting.melted$Time <- as.factor(exiting.melted$Time)

exiting.melted <- exiting.melted[order(exiting.melted$Time, exiting.melted$Station),]


# And merge everything.
all.melted <- cbind(entering.melted, exiting.melted$PassOut)
colnames(all.melted)[4] <- "PassOut"
all.melted <- add_column(all.melted, "Direction", .before = "Time")
colnames(all.melted)[1] <- "Direction"
all.melted$Direction <- 0
all.melted$Time <- levels(droplevels((all.melted$Time)))


all.melted <- separate(data = all.melted, col = Time, into = c("Date", "Time"), sep = " ")
all.melted$Date <- NULL
all.melted <- all.melted[order(all.melted$Time, all.melted$Station),]
all.melted <- add_column(all.melted, "To", .after = "Time")
colnames(all.melted)[3] <- "To"
all.melted$From <- 0

for (i in 1:nrow(all.melted)) {
    # split the string first.
    splitted <- unlist(strsplit(all.melted[i,]$Time, ":"))
    
    from.time <- as.numeric(splitted[1])
    
    if (from.time < 7) {
        all.melted[i,]$From <- 6
        all.melted[i,]$To <- from.time + 1
    } else if (from.time >= 7 && from.time < 9) {
        all.melted[i,]$From <- 7
        all.melted[i,]$To <- 9
    } else if (from.time >= 9 && from.time < 16) {
        all.melted[i,]$From <- 9
        all.melted[i,]$To <- 16
    } else if (from.time >= 16 && from.time < 18) {
        all.melted[i,]$From <- 16
        all.melted[i,]$To <- 18
    } else if (from.time >= 18) {
        all.melted[i,]$From <- 18
        all.melted[i,]$To <- 21.5
    }
    
    all.melted[i,]$Time <- from.time
}

all.melted$Time <- NULL

xxx <- aggregate(PassIn~Direction+From+To+Station, all.melted, sum)
uuu <- aggregate(PassOut~Direction+From+To+Station, all.melted, sum)

all.melted.1 <- cbind(xxx, uuu)




all.melted.pt1 <- aggregate(PassIn ~ Direction + From + To + Station, data=all.melted.1, sum)
all.melted.pt2 <- aggregate(PassOut ~ From + To + Station, data=all.melted.1, sum)

all.melted <- cbind(all.melted.pt1, all.melted.pt2$PassOut)
colnames(all.melted)[6] <- "PassOut" 
colnames(all.melted)[4] <- "Stop"

all.melted$Stop <- factor(all.melted$Stop, levels = stations)
all.melted$To <- as.numeric(all.melted$To)


all.melted <- all.melted[order(all.melted$To, factor(all.melted$Stop)),]
all.melted$PassIn <- ceiling(all.melted$PassIn)
all.melted$PassOut <- ceiling(all.melted$PassOut)

write.csv(all.melted[, c(4,1,2,3,5,6)], file = paste("12a.pr_to_cs.csv", sep = ""), sep = ';', quote = FALSE)


