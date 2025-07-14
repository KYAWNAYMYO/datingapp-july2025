using API.Entities;

namespace API.Interfaces;

public interface IMemberRepository
{
    void Update(Member member);

    Task<bool> SaveAllAsync();

    Task<IReadOnlyList<Member>> GetMembersAsynce();

    Task<Member?> GetMemberByIdAsync(string id);

    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);
}
