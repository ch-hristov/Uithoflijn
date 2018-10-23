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
library(tibble)
# Plotting libraries

file.name <- '12b'

# Load the file
all.content = readLines(paste(file.name,".csv", sep = ''))
skip.first = all.content[-1]
passenger.info.route.A = read.csv(textConnection(skip.first), header = TRUE, stringsAsFactors = FALSE)

passenger.info.route.A$Stadion.Galgenwaard <- ceiling((passenger.info.route.A$Stadion.Galgenwaard + passenger.info.route.A$Rubenslaan) / 2)
passenger.info.route.A$Rubenslaan <- NULL

passenger.info.route.A$Bleekstraat <- ceiling((passenger.info.route.A$Bleekstraat + passenger.info.route.A$Sterrenwijk) / 2)
passenger.info.route.A$Sterrenwijk <- NULL

passenger.info.route.A$AZU <- passenger.info.route.A$AZU.1

# Create the UMC column for 
passenger.info.route.A <- add_column(passenger.info.route.A, "UMC", .after = "AZU")
colnames(passenger.info.route.A)[11] <- "UMC"
passenger.info.route.A <- add_column(passenger.info.route.A, "WKZ", .after = "UMC")
colnames(passenger.info.route.A)[12] <- "WKZ"
passenger.info.route.A <- add_column(passenger.info.route.A, "P+R Uithof", .after = "WKZ")
colnames(passenger.info.route.A)[13] <- "P+R Uithof"


passenger.info.route.A <- add_column(passenger.info.route.A, "UMC.1", .after = "AZU.1")
colnames(passenger.info.route.A)[23] <- "UMC.1"
passenger.info.route.A <- add_column(passenger.info.route.A, "WKZ.1", .after = "UMC.1")
colnames(passenger.info.route.A)[24] <- "WKZ.1"
passenger.info.route.A <- add_column(passenger.info.route.A, "P+R Uithof.1", .after = "WKZ.1")
colnames(passenger.info.route.A)[25] <- "P+R Uithof.1"


passenger.info.route.A$UMC <- ceiling(passenger.info.route.A$AZU * (3230/3904))
passenger.info.route.A$WKZ <- ceiling(passenger.info.route.A$AZU * (659/3904))
passenger.info.route.A$`P+R Uithof` <- ceiling(passenger.info.route.A$AZU * (15/3904))
passenger.info.route.A$`P+R Uithof` <- 0

passenger.info.route.A$AZU <- NULL

passenger.info.route.A$UMC.1 <- ceiling(passenger.info.route.A$AZU.1 * (2577/3236))
passenger.info.route.A$WKZ.1 <- ceiling(passenger.info.route.A$AZU.1 * (644/3236))
passenger.info.route.A$`P+R Uithof.1` <- ceiling(passenger.info.route.A$AZU.1 * (15/3236))

passenger.info.route.A$AZU.1 <- NULL
# write.csv(passenger.info.route.A, file = paste(file.name, ".adjusted.csv", sep = ""), quote = FALSE)

# Now we need to get the averages per trip
averages.per.trip <- format(aggregate(passenger.info.route.A[-c(1, 2, 3)], list(passenger.info.route.A$Departure), mean), digits = 2)
averages.per.trip$Group.1 <- times(paste(averages.per.trip$Group.1, "00", sep = ":"))

exiting.passengers <- averages.per.trip[, c(1, 11:21)]


# ENTERING PASSENGERS CS -> P+R
df2 <- averages.per.trip[, c(1:10)]
df2$Group.1[125:nrow(df2)] <- paste("0",df2$Group.1[125:nrow(df2)], sep = "")
df2$Group.1 <- paste("20180930 ", gsub(":", "", df2$Group.1), "00", sep = "")
df2$Time <- as.POSIXct(df2$Group.1, format = "%Y%m%d %H%M%S")
df2$by15 = cut(df2$Time, breaks=c("15 min"))
df2$CS.Centrumzijde <- as.numeric(df2$CS.Centrumzijde)
df2$Bleekstraat <- as.numeric(df2$Bleekstraat)
df2$Stadion.Galgenwaard <- as.numeric(df2$Stadion.Galgenwaard)
df2$De.Kromme.Rijn <- as.numeric(df2$De.Kromme.Rijn)
df2$Padualaan <- as.numeric(df2$Padualaan)
df2$Heidelberglaan <- as.numeric(df2$Heidelberglaan)
df2$UMC <- as.numeric(df2$UMC)
df2$WKZ <- as.numeric(df2$WKZ)
df2$`P+R Uithof` <- as.numeric(df2$`P+R Uithof`)


entering.summary <- c()
entering.summary$CS.Centrumzijde <- aggregate(CS.Centrumzijde ~ by15, FUN=sum, data=df2)
entering.summary$Bleekstraat <- aggregate(Bleekstraat ~ by15, FUN=sum, data=df2)
entering.summary$Stadion.Galgenwaard <- aggregate(Stadion.Galgenwaard ~ by15, FUN=sum, data=df2)
entering.summary$De.Kromme.Rijn <- aggregate(De.Kromme.Rijn ~ by15, FUN=sum, data=df2)
entering.summary$Padualaan <- aggregate(Padualaan ~ by15, FUN=sum, data=df2)
entering.summary$Heidelberglaan <- aggregate(Heidelberglaan ~ by15, FUN=sum, data=df2)
entering.summary$UMC <- aggregate(UMC ~ by15, FUN=sum, data=df2)
entering.summary$WKZ <- aggregate(WKZ ~ by15, FUN=sum, data=df2)
entering.summary$`P+R Uithof` <- aggregate(`P+R Uithof` ~ by15, FUN=sum, data=df2)



# Do the same for the exiting passengers CS -> P+R
df3 <- exiting.passengers
df3$Group.1[125:nrow(df3)] <- paste("0",df3$Group.1[125:nrow(df3)], sep = "")
df3$Group.1 <- paste("20180930 ", gsub(":", "", df3$Group.1), "00", sep = "")
df3$Time <- as.POSIXct(df3$Group.1, format = "%Y%m%d %H%M%S")
df3$by15 = cut(df3$Time, breaks=c("60 min"))
df3$`CS.Centrumzijde.1` <- as.numeric(df3$`CS.Centrumzijde.1`)
df3$WKZ.1 <- as.numeric(df3$WKZ.1)
df3$UMC.1 <- as.numeric(df3$UMC.1)
df3$Heidelberglaan.1 <- as.numeric(df3$Heidelberglaan.1)
df3$Padualaan.1 <- as.numeric(df3$Padualaan.1)
df3$De.Kromme.Rijn.1 <- as.numeric(df3$De.Kromme.Rijn.1)
df3$Stadion.Galgenwaard.1 <- as.numeric(df3$Stadion.Galgenwaard.1)
df3$Bleekstraat.1 <- as.numeric(df3$Bleekstraat.1)
df3$`P+R Uithof.1` <- as.numeric(df3$`P+R Uithof.1`)



exiting.summary <- c()
exiting.summary$CS.Centrumzijde.1 <- aggregate(CS.Centrumzijde.1 ~ by15, FUN=sum, data=df3)
exiting.summary$Bleekstraat.1 <- aggregate(Bleekstraat.1 ~ by15, FUN=sum, data=df3)
exiting.summary$Stadion.Galgenwaard.1 <- aggregate(Stadion.Galgenwaard.1 ~ by15, FUN=sum, data=df3)
exiting.summary$De.Kromme.Rijn.1 <- aggregate(De.Kromme.Rijn.1 ~ by15, FUN=sum, data=df3)
exiting.summary$Padualaan.1 <- aggregate(Padualaan.1 ~ by15, FUN=sum, data=df3)
exiting.summary$Heidelberglaan.1 <- aggregate(Heidelberglaan.1 ~ by15, FUN=sum, data=df3)
exiting.summary$UMC.1 <- aggregate(UMC.1 ~ by15, FUN=sum, data=df3)
exiting.summary$WKZ.1 <- aggregate(WKZ.1 ~ by15, FUN=sum, data=df3)
exiting.summary$`P+R Uithof.1` <- aggregate(`P+R Uithof.1` ~ by15, FUN=sum, data=df3)


stations <- rev(c("P+R Uithof", "WKZ", "UMC","Heidelberglaan", "Padualaan","Kromme Rijn",
              "Galgenwaard", "Vaartscherijn","Centrumzijde Centraal Station"))

temp <- as.data.frame(entering.summary)
temp[, c(3,5,7,9,11,13,15,17)] <- NULL
colnames(temp) <- c("by15", rev(stations))


entering.summary <- as.data.frame(entering.summary)
entering.summary[, c(3,5,7,9,11,13,15,17)] <- NULL
colnames(entering.summary) <- c("by15", stations)
write.csv(entering.summary, file = paste(file.name, ".entering.15.intervals.csv", sep = ""), quote = FALSE)

exiting.summary <- as.data.frame(exiting.summary)
exiting.summary[, c(3,5,7,9,11,13,15,17)] <- NULL
colnames(exiting.summary) <- c("by15", stations)


#### Bring to format

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
all.melted$Direction <- 1
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

write.csv(all.melted[, c(4,1,2,3,5,6)], file = paste("12b.cs_to_pr.csv", sep = ""), sep = ';', quote = FALSE)



