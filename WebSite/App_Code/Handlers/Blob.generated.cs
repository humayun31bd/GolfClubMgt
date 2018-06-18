namespace MyCompany.Handlers
{
    
    
    public partial class BlobFactoryConfig : BlobFactory
    {
        
        public static void Initialize()
        {
            // register blob handlers
            RegisterHandler("BallBoyPicture", "\"dbo\".\"BallBoy\"", "\"Picture\"", new string[] {
                        "\"BallBoyID\""}, "Ball Boy Picture", "BallBoy", "Picture");
            RegisterHandler("CaddiePicture", "\"dbo\".\"Caddie\"", "\"Picture\"", new string[] {
                        "\"CaddieID\""}, "Caddie Picture", "Caddie", "Picture");
            RegisterHandler("MemberChildrenPic", "\"dbo\".\"MemberChildren\"", "\"Pic\"", new string[] {
                        "\"MemberChildID\""}, "Member Children Pic", "MemberChildren", "Pic");
            RegisterHandler("MemberInfoPhoto", "\"dbo\".\"MemberInfo\"", "\"Photo\"", new string[] {
                        "\"MemberID\""}, "Member Info Photo", "MemberInfo", "Photo");
            RegisterHandler("MemberSpouseSpousePic", "\"dbo\".\"MemberSpouse\"", "\"SpousePic\"", new string[] {
                        "\"MemberSpouseID\""}, "Member Spouse Spouse Pic", "MemberSpouse", "SpousePic");
        }
    }
}
