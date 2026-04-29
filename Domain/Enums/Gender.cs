namespace Domain.Enums
{
        /// <summary>
        /// Represents a comprehensive range of gender identities and expressions. 
        /// Used for inclusive data collection and personalized user experiences within the domain.
        /// </summary>
        public enum Gender
        {
                // --- The "Basics" ---

                /// <summary> Identifies as a man. </summary>
                Man,
                /// <summary> Identifies as a woman. </summary>
                Woman,
                /// <summary> The user has explicitly chosen not to disclose their gender identity. </summary>
                PreferNotToSay,

                // --- Spectrum and Inclusive Identities ---

                /// <summary> Identifying as having no gender or a neutral gender identity. </summary>
                Agender,
                /// <summary> An identity that exists outside the traditional binary; often used synonymously with non-binary. </summary>
                Abinary,
                /// <summary> A gender identity that is separate from the concepts of man or woman, but still defined. </summary>
                Aliagender,
                /// <summary> A gender identity that feels like a mix of various gender traits. </summary>
                Amalgagender,
                /// <summary> A person whose identity is a blend of both masculine and feminine traits. </summary>
                Androgyne,
                /// <summary> A gender identity away from the binary, yet with a specific gendered feeling. </summary>
                Aporagender,
                /// <summary> A cultural gender identity in the Philippines, often assigned male at birth but expressing feminine traits. </summary>
                Bakla,
                /// <summary> Identifying as two distinct genders, either simultaneously or at different times. </summary>
                Bigender,
                /// <summary> A term used to describe a person (often a woman) whose gender expression is masculine. </summary>
                Butch,
                /// <summary> A person whose gender identity matches the sex they were assigned at birth. </summary>
                Cisgender,
                /// <summary> A man whose gender identity matches the male sex assigned at birth. </summary>
                CisMan,
                /// <summary> A woman whose gender identity matches the female sex assigned at birth. </summary>
                CisWoman,
                /// <summary> An umbrella term for identities that have a partial connection to a certain gender. </summary>
                Demigender,
                /// <summary> Identifying partially, but not wholly, as a boy or man. </summary>
                Demiboy,
                /// <summary> Identifying partially, but not wholly, as a girl or woman. </summary>
                Demigirl,
                /// <summary> A gender identity that is partially fluid and partially static. </summary>
                Demiflux,
                /// <summary> A phonetic spelling of "NB" (Non-Binary); an identity outside the gender binary. </summary>
                Enby,
                /// <summary> A third-gender identity in Samoan culture, assigned male at birth but identifying with feminine roles. </summary>
                FaAfafine,
                /// <summary> A person whose gender expression is feminine, regardless of their gender identity. </summary>
                Femme,
                /// <summary> A gender identity that changes over time, moving between different points on the spectrum. </summary>
                Genderfluid,
                /// <summary> A gender identity where the intensity of the gender changes over time. </summary>
                Genderflux,
                /// <summary> A person who does not subscribe to conventional gender distinctions. </summary>
                Genderqueer,
                /// <summary> People who do not follow other people's ideas or stereotypes about how they should look or act based on the female or male sex they were assigned at birth. </summary>
                GenderNonConforming,
                /// <summary> A person whose gender identity or expression differs from what is typically associated with the sex they were assigned at birth. </summary>
                GenderVariant,
                /// <summary> An umbrella term for people who expand their own culture's commonly held definitions of gender. </summary>
                GenderExpansive,
                /// <summary> A person who has a weak or vague connection to the concept of gender. </summary>
                Graygender,
                /// <summary> A social group or third gender recognized in South Asian cultures. </summary>
                Hijra,
                /// <summary> A gender identity that is between or a mix of binary genders. </summary>
                Intergender,
                /// <summary> A feminine gender identity that is separate from "woman" but "near" it. </summary>
                Juxera,
                /// <summary> A third-gender identity in Oman and the Arabian Peninsula. </summary>
                Khanith,
                /// <summary> A gender identity that is mostly agender but with a connection to a specific gender. </summary>
                Libragender,
                /// <summary> Mostly agender, but with a partial connection to masculinity. </summary>
                Libraboy,
                /// <summary> Mostly agender, but with a partial connection to femininity. </summary>
                Libragirl,
                /// <summary> A traditional third-gender identity in Kanaka Maoli (Hawaiian) and Tahitian cultures. </summary>
                Mahu,
                /// <summary> A gender identity that is intentionally "other," characterized by being autonomous and non-binary. </summary>
                Maverique,
                /// <summary> Identifying as multiple genders at once or over time. </summary>
                Multigender,
                /// <summary> A third-gender identity in the Zapotec cultures of Oaxaca, Mexico. </summary>
                Muxe,
                /// <summary> A neutral or null gender identity. </summary>
                Neutrois,
                /// <summary> An identity that does not fit into the categories of male or female. </summary>
                NonBinary,
                /// <summary> A gender identity that is complicated or difficult to describe using existing language. </summary>
                Novigender,
                /// <summary> Identifying as all genders within one's own life/culture. </summary>
                Omnigender,
                /// <summary> Identifying as all possible genders. </summary>
                Pangender,
                /// <summary> A gender identity that is very near a certain gender, but not quite it. </summary>
                Paragender,
                /// <summary> Identifying as many, but not necessarily all, genders. </summary>
                Polygender,
                /// <summary> A masculine gender identity that is separate from "man" but "near" it. </summary>
                Proxvir,
                /// <summary> Currently exploring or unsure of one's gender identity. </summary>
                Questioning,
                /// <summary> A category for people who do not identify as male or female. </summary>
                ThirdGender,
                /// <summary> An umbrella term for people whose gender identity differs from the sex they were assigned at birth. </summary>
                Transgender,
                /// <summary> A man who was assigned female at birth. </summary>
                TransMan,
                /// <summary> A woman who was assigned male at birth. </summary>
                TransWoman,
                /// <summary> A person who identifies with femininity but was assigned male at birth. </summary>
                Transfeminine,
                /// <summary> A person who identifies with masculinity but was assigned female at birth. </summary>
                Transmasculine,
                /// <summary> Identifying as three distinct genders. </summary>
                Trigender,
                /// <summary> A modern, pan-Indian term used by some Indigenous North Americans to describe their sexual, gender and/or spiritual identity. </summary>
                TwoSpirit,
                /// <summary> A gender identity that cannot be described by human gender concepts, often using non-human metaphors. </summary>
                Xenogender,

                // --- System Fallbacks ---

                /// <summary> The user identifies with a gender not specifically listed above </summary>
                Other
        }
}
